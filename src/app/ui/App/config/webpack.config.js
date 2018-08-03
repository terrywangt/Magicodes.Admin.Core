const path = require('path');
const webpackConfig = require('@ionic/app-scripts/config/webpack.config');
const fs = require('fs');
const util = require('./build/util');
const tsconfig = require('../tsconfig.json');
const BUILD_ENV=process.env.BUILD_ENV||'dev';
const BUILD_TARGET=process.env.BUILD_TARGET||'app';

//配置paths 
let aliases = {};
let pathKeyArray = Object.keys(tsconfig.compilerOptions.paths);

if(BUILD_TARGET=='android'){
  util.addLibsToGradle();
  // util.addVersionToGradle();
}
pathKeyArray.forEach(currentPath => {
  let correctPath = currentPath.replace("/*", "");
  let currentPathValue = tsconfig.compilerOptions.paths[currentPath][0].replace('*', '');
  aliases[correctPath] = path.resolve(tsconfig.compilerOptions.baseUrl + '/' + currentPathValue);
  aliases['@env']=path.resolve(environmentPath(BUILD_ENV));
})
// prod build 
if (process.env.IONIC_ENV === 'prod') {
  webpackConfig.prod.output.filename = '[name].[chunkhash:8].js';
  webpackConfig.prod.plugins.push(
    new IndexFileUpdaterPlugin()
  );
}
webpackConfig.dev.resolve = webpackConfig.prod.resolve = {
  extensions: ['.ts', '.js', '.json'],
  modules: [
    path.resolve('node_modules'),
    path.resolve(tsconfig.compilerOptions.baseUrl)
  ],
  alias: aliases
}
// 
function IndexFileUpdaterPlugin(options) { };
IndexFileUpdaterPlugin.prototype.apply = (compiler)=> {
  compiler.plugin('done', stats=> {
    

    let html = util.getIndexHtml();
    const fileType = 'js';
    html = util.appendOrOverwriteHashJsValues(html, 'build/main', util.getHashForFile('main', fileType));
    html = util.appendOrOverwriteHashJsValues(html, 'build/vendor', util.getHashForFile('vendor', fileType));
    if(BUILD_TARGET=='web'){
      html=util.appendOrOverwriteHashJsValues(html,'cordova','')
    }
    util.writeIndexHtml(html);
    let swjs = util.getSWJS();
    swjs = util.appendOrOverwriteSwjs(swjs, 'main', util.getHashForFile('main', fileType));
    swjs = util.appendOrOverwriteSwjs(swjs, 'vendor', util.getHashForFile('vendor', fileType));
    util.writeSWJS(swjs);
  });
};


function environmentPath(env) {
  let pathUrl='./src/environments/environment';
  let filePath = pathUrl + (env === 'prod' ? '.prod' : '') + '.ts';
  if (!fs.existsSync(filePath)) {
    console.log('\n' + filePath + ' does not exist!');
  } else {
    return filePath;
  }
};

module.exports = ()=> {
  return webpackConfig;
};