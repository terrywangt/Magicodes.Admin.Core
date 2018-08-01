using System.IO;
using Magicodes.Admin.Configuration;
using Magicodes.Storage.Core;
using Magicodes.Storage.Local.Core;
using Microsoft.AspNetCore.Hosting;

namespace Magicodes.Unity.Storage
{
    /// <inheritdoc />
    public class StorageManager : IStorageManager
    {
        private readonly IAppConfigurationAccessor _appConfiguration;
        private readonly IHostingEnvironment _env;

        public StorageManager(IAppConfigurationAccessor appConfiguration, IHostingEnvironment env)
        {
            _appConfiguration = appConfiguration;
            _env = env;
        }

        /// <inheritdoc />
        public IStorageProvider StorageProvider { get; set; }


        /// <summary>
        /// 根据配置初始化存储提供程序
        /// </summary>
        public void Initialize()
        {
            #region 配置存储程序
            switch (_appConfiguration.Configuration["StorageProvider:Type"])
            {
                case "LocalStorageProvider":
                {
                    var rootPath = _appConfiguration.Configuration["StorageProvider:LocalStorageProvider:RootPath"];
                    if (!rootPath.Contains(":"))
                    {
                        rootPath = Path.Combine(_env.WebRootPath, rootPath);
                    }

                    if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);

                    StorageProvider = new LocalStorageProvider(rootPath, _appConfiguration.Configuration["StorageProvider:LocalStorageProvider:RootUrl"]);
                    break;
                }
                //TODO:阿里云、腾讯云支持
                default:
                    break;
            }
            #endregion
        }
    }
}
