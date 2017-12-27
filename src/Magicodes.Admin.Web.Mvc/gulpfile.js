/// <binding BeforeBuild='clean' Clean='clean' />
"use strict";

var gulp = require("gulp"),
    async = require('async'),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    merge = require("merge-stream"),
    rimraf = require("rimraf"),
    gutil = require('gulp-util'),
    cleanCSS = require('gulp-clean-css'),
    runSequence = require('run-sequence'),
    bundleconfig = require("./bundle.config.js");

var regex = {
    css: /\.css$/,
    js: /\.js$/
};

gulp.task("min", ["min:js", "min:css"]);

gulp.task("min:js", function () {
    var tasks = getBundles(regex.js).map(function (bundle) {
        var outputFileName = getOutputFileName(bundle.outputFileName);
        var outputFolder = getOutputFolder(bundle.outputFileName);

        return gulp.src(bundle.inputFiles)
            .pipe(concat(outputFileName))
            .pipe(gulp.dest(outputFolder));
    });

    if (gutil.env.prod) {
        var minifyTasks = getBundles(regex.js).map(function (bundle) {
            var outputFileName = getOutputFileName(bundle.outputFileName);
            var outputFolder = getOutputFolder(bundle.outputFileName);

            if (bundle.createMinified === false || outputFileName.endsWith('.min.js')) {
                return null;
            }

            var minifiedJsOutputFile = outputFileName.substr(0, outputFileName.lastIndexOf(".")) + ".min.js";
            return gulp.src(bundle.inputFiles)
                .pipe(concat(minifiedJsOutputFile))
                .pipe(uglify())
                .pipe(gulp.dest(outputFolder));
        });

        for (var i = 0; i < minifyTasks.length; i++) {
            if (minifyTasks[i] == null) {
                continue;
            }

            tasks.push(minifyTasks[i]);
        }
    }

    return merge(tasks);
});

gulp.task("min:css", function () {
    var tasks = getBundles(regex.css).map(function (bundle) {
        var outputFolder = getOutputFolder(bundle.outputFileName);
        var outputFileName = getOutputFileName(bundle.outputFileName);

        return gulp.src(bundle.inputFiles)
            .pipe(cleanCSS({
                level: 0, // no optimization on css file
                rebaseTo: outputFolder
            }))
            .pipe(concat(outputFileName))
            .pipe(gulp.dest(outputFolder));
    });

    if (gutil.env.prod) {
        var minifyTasks = getBundles(regex.css).map(function (bundle) {
            var outputFolder = getOutputFolder(bundle.outputFileName);
            var outputFileName = getOutputFileName(bundle.outputFileName);

            if (bundle.createMinified === false || outputFileName.endsWith('.min.css')) {
                return null;
            }

            var minifiedCssOutputFile = outputFileName.substr(0, outputFileName.lastIndexOf(".")) + ".min.css";
            return gulp.src(bundle.inputFiles)
                .pipe(cleanCSS({
                    rebaseTo: outputFolder,
                    level: 1 // default optimization on css file
                }))
                .pipe(concat(minifiedCssOutputFile))
                .pipe(gulp.dest(outputFolder));
        });

        for (var i = 0; i < minifyTasks.length; i++) {
            if (minifyTasks[i] == null) {
                continue;
            }

            tasks.push(minifyTasks[i]);
        }
    }

    return merge(tasks);
});

gulp.task("clean_bundles", function (cb) {
    async.parallel(bundleconfig.bundles.map(function (bundle) {
        return function (done) {
            if (bundle.outputFileName.match(/[^/]+(css|js)$/)) {
                rimraf(bundle.outputFileName, done);
            } else {
                rimraf(bundle.outputFileName + '/*', done);
            }
        }
    }), cb);
});

gulp.task("watch", function () {
    getBundles(regex.js).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, ["min:js"]);
    });

    getBundles(regex.css).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, ["min:css"]);
    });

    gulp.watch('./bundle.config.js', function () {
        runSequence(['min:css', 'min:js']);
    });
});

gulp.task('copy:node_modules', function () {
    rimraf.sync(bundleconfig.libsFolder + '/**/*', { force: true });
    var tasks = [];

    for (var mapping in bundleconfig.mappings) {
        if (bundleconfig.mappings.hasOwnProperty(mapping)) {
            var destination = bundleconfig.libsFolder + '/' + bundleconfig.mappings[mapping];
            if (mapping.match(/[^/]+(css|js)$/)) {
                tasks.push(
                    gulp.src(mapping).pipe(gulp.dest(destination))
                );
            } else {
                tasks.push(
                    gulp.src(mapping + '/**/*').pipe(gulp.dest(destination))
                );
            }
        }
    }

    return merge(tasks);
});

gulp.task('default', ['copy:node_modules'], function () {
    runSequence('watch', ['min:css', 'min:js']);
});

function getBundles(regexPattern) {
    return bundleconfig.bundles.filter(function (bundle) {
        return regexPattern.test(bundle.outputFileName);
    });
}

function getOutputFileName(fullFilePath) {
    var lastIndexOfSlash = fullFilePath.lastIndexOf('/');
    return fullFilePath.substr(lastIndexOfSlash, fullFilePath.length - lastIndexOfSlash);
}

function getOutputFolder(fullFilePath) {
    var lastIndexOfSlash = fullFilePath.lastIndexOf('/');
    return fullFilePath.substr(0, lastIndexOfSlash);
}