const fs = require('fs');


const util = module.exports = {}

const wwwDirPath = 'www';
const buildDirPath = 'www/build';

/* This could likely be pulled from Webpack stats, but this'll do for now ... */
util.getHashForFile = function (fileName, fileType) {
  const result = fs.readdirSync(buildDirPath).filter(file => {
    return file.startsWith(fileName) && file.endsWith(fileType) && (file.indexOf('map') === -1)
  })[0];
  return result.split('.')[1];
}

util.appendOrOverwriteHashJsValues = function (indexString, chunkName, hash) {
  let match = '(<script src="' + chunkName + '*.js"></script>)';
  let regexp = indexString.match(match)[0];
  let replace = '<script src="' + chunkName + '.' + hash + '.js"></script>';
  if (!hash) {
    replace = '';
  }
  return indexString.replace(regexp, replace);
}

util.appendOrOverwriteHashCssValues = function (indexString, chunkName, hash) {
  let match = '(<link href="build/' + chunkName + '.*.css" rel="stylesheet">)';
  let regexp = indexString.match(match)[0];
  const replace = '<link href="build/' + chunkName + '.' + hash + '.css" rel="stylesheet">';
  return indexString.replace(regexp, replace);
}



util.getIndexHtml = function () {
  return getString(wwwDir('index.html'))
}

util.writeIndexHtml = function (htmlString) {
  setString(wwwDir('index.html'), htmlString);
}
/**
 * service-worker相关操作
 */
util.appendOrOverwriteSwjs = function (indexString, name, hash) {
  let match = name + '.js';
  let regexp = indexString.match(match)[0];
  const replace = name + '.' + hash + '.js';
  return indexString.replace(regexp, replace);
}
util.getSWJS = function () {
  return getString(wwwDir('service-worker.js'))
}

util.writeSWJS = function (htmlString) {
  setString(wwwDir('service-worker.js'), htmlString);
}

/**
 * 修复so包加载问题
 */
util.addLibsToGradle = function () {
  let localPath = 'platforms/android/app/build.gradle';
  let match = 'sourceSets';
  let replaceString = `
  android {
    sourceSets {
        main {
           jniLibs.srcDirs = ['libs']
        }
    }`;
  if (fs.existsSync(localPath)) {
    let build = getString(localPath);
    if (build.indexOf(match) == -1) {
      setString(localPath, build.replace('android {', replaceString))
    }
  }
}
/**
 * 修复gradle 报错问题
 */
util.addVersionToGradle = function () {
  let localPaths = ['platforms/android/app/build.gradle','platforms/android/build.gradle'];
  let match = 'resolutionStrategy';
  let replaceString = `
  configurations.all {
    resolutionStrategy {
        force 'com.android.support:support-v4:27.1.0'
    }}`;
    localPaths.forEach(localPath=>{
      if (fs.existsSync(localPath)) {
        let build = getString(localPath);
        if (build.indexOf(match) == -1) {
          setString(localPath, build+replaceString)
        }
      }
    })
}


getString = function (name) {
  return fs.readFileSync(name).toString();
}
setString = function (name, htmlString) {
  return fs.writeFileSync(name, htmlString);
}

buildDir = function (file) {
  return buildDirPath + '/' + file;
}

wwwDir = function (file) {
  return wwwDirPath + '/' + file;
}
