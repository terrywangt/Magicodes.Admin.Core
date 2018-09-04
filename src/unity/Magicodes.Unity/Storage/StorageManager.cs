using System.IO;
using Abp.Extensions;
using Abp.UI;
using Magicodes.Admin.Configuration;
using Magicodes.Storage.AliyunOss.Core;
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
                case "AliyunOssStorageProvider":
                    {
                        var accessKeyId =
                            _appConfiguration.Configuration["StorageProvider:AliyunOssStorageProvider:AccessKeyId"];
                        var accessKeySecret =
                            _appConfiguration.Configuration["StorageProvider:AliyunOssStorageProvider:AccessKeySecret"];
                        var endpoint =
                            _appConfiguration.Configuration["StorageProvider:AliyunOssStorageProvider:Endpoint"];
                        if (accessKeyId.IsNullOrWhiteSpace())
                        {
                            throw new UserFriendlyException("AliyunOssStorageProvider accessKeyId is null!");
                        }
                        if (accessKeySecret.IsNullOrWhiteSpace())
                        {
                            throw new UserFriendlyException("AliyunOssStorageProvider accessKeySecret is null!");
                        }
                        if (endpoint.IsNullOrWhiteSpace())
                        {
                            throw new UserFriendlyException("AliyunOssStorageProvider endpoint is null!");
                        }

                        var aliyunOssConfig = new AliyunOssConfig
                        {
                            AccessKeyId = accessKeyId,
                            AccessKeySecret = accessKeySecret,
                            Endpoint = endpoint
                        };
                        StorageProvider = new AliyunOssStorageProvider(aliyunOssConfig);
                        break;
                    }
                //TODO:腾讯云支持
                default:
                    break;
            }
            #endregion
        }
    }
}
