using Abp.Configuration;
using Abp.Extensions;
using Abp.UI;
using Castle.Core.Logging;
using Magicodes.Admin.Configuration;
using Magicodes.Storage.AliyunOss.Core;
using Magicodes.Storage.Core;
using Magicodes.Storage.Local.Core;
using Magicodes.Storage.Tencent.Core;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace Magicodes.Unity.Storage
{
    /// <inheritdoc />
    public class StorageManager : IStorageManager
    {
        private readonly IAppConfigurationAccessor _appConfiguration;
        private readonly IHostingEnvironment _env;
        private readonly ISettingManager _settingManager;

        public ILogger Logger { get; set; }

        public StorageManager()
        {
            Logger = NullLogger.Instance;
        }

        public StorageManager(IAppConfigurationAccessor appConfiguration, IHostingEnvironment env, ISettingManager settingManager)
        {
            _appConfiguration = appConfiguration;
            _env = env;
            _settingManager = settingManager;
        }

        /// <inheritdoc />
        public IStorageProvider StorageProvider { get; set; }


        /// <summary>
        /// 根据配置初始化存储提供程序
        /// </summary>
        public void Initialize()
        {
            //日志函数
            //void LogAction(string tag, string message)
            //{
            //    if (tag.Equals("error", StringComparison.CurrentCultureIgnoreCase))
            //        Logger.Error(message);
            //    else
            //        Logger.Debug(message);
            //}

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
                        AliyunOssConfig aliyunOssConfig;

                        if (Convert.ToBoolean(_settingManager.GetSettingValueAsync(AppSettings.AliStorageManagement.IsEnabled).Result))
                        {
                            aliyunOssConfig = new AliyunOssConfig()
                            {
                                AccessKeyId = _settingManager.GetSettingValueAsync(AppSettings.AliStorageManagement.AccessKeyId).Result,
                                AccessKeySecret = _settingManager.GetSettingValueAsync(AppSettings.AliStorageManagement.AccessKeySecret).Result,
                                Endpoint = _settingManager.GetSettingValueAsync(AppSettings.AliStorageManagement.EndPoint).Result,
                                BucketName = _settingManager.GetSettingValueAsync(AppSettings.AliStorageManagement.BucketName).Result
                            };
                        }
                        else
                        {
                            aliyunOssConfig = new AliyunOssConfig()
                            {
                                AccessKeyId = _appConfiguration.Configuration["StorageProvider:AliyunOssStorageProvider:AccessKeyId"],
                                AccessKeySecret = _appConfiguration.Configuration["StorageProvider:AliyunOssStorageProvider:AccessKeySecret"],
                                Endpoint = _appConfiguration.Configuration["StorageProvider:AliyunOssStorageProvider:Endpoint"],
                                BucketName = _appConfiguration.Configuration["StorageProvider:AliyunOssStorageProvider:BucketName"]
                            };
                        }

                        if (aliyunOssConfig.AccessKeyId.IsNullOrWhiteSpace())
                        {
                            throw new UserFriendlyException("AliyunOssStorageProvider accessKeyId is null!");
                        }
                        if (aliyunOssConfig.AccessKeySecret.IsNullOrWhiteSpace())
                        {
                            throw new UserFriendlyException("AliyunOssStorageProvider accessKeySecret is null!");
                        }
                        if (aliyunOssConfig.Endpoint.IsNullOrWhiteSpace())
                        {
                            throw new UserFriendlyException("AliyunOssStorageProvider endpoint is null!");
                        }
                        if (aliyunOssConfig.BucketName.IsNullOrWhiteSpace())
                        {
                            throw new UserFriendlyException("AliyunOssStorageProvider bucketName is null!");
                        }
                        StorageProvider = new AliyunOssStorageProvider(aliyunOssConfig);
                        break;
                    }
                case "TencentCosStorageProvider":
                    {
                        TencentCosConfig tencentCosConfig;

                        if (Convert.ToBoolean(_settingManager.GetSettingValueAsync(AppSettings.TencentStorageManagement.IsEnabled).Result))
                        {
                            tencentCosConfig = new TencentCosConfig()
                            {
                                AppId = _settingManager.GetSettingValueAsync(AppSettings.TencentStorageManagement.AppId).Result,
                                BucketName = _settingManager.GetSettingValueAsync(AppSettings.TencentStorageManagement.BucketName).Result,
                                Region = _settingManager.GetSettingValueAsync(AppSettings.TencentStorageManagement.Region).Result,
                                SecretId = _settingManager.GetSettingValueAsync(AppSettings.TencentStorageManagement.SecretId).Result,
                                SecretKey = _settingManager.GetSettingValueAsync(AppSettings.TencentStorageManagement.SecretKey).Result
                            };
                        }
                        else
                        {
                            tencentCosConfig = new TencentCosConfig()
                            {
                                AppId = _appConfiguration.Configuration["StorageProvider:TencentCosStorageProvider:AppId"],
                                BucketName = _appConfiguration.Configuration["StorageProvider:TencentCosStorageProvider:BucketName"],
                                Region = _appConfiguration.Configuration["StorageProvider:TencentCosStorageProvider:Region"],
                                SecretId = _appConfiguration.Configuration["StorageProvider:TencentCosStorageProvider:SecretId"],
                                SecretKey = _appConfiguration.Configuration["StorageProvider:TencentCosStorageProvider:SecretKey"]
                            };
                        }

                        if (tencentCosConfig.AppId.IsNullOrWhiteSpace())
                        {
                            throw new UserFriendlyException("TencentCosStorageProvider AppId is null!");
                        }
                        if (tencentCosConfig.BucketName.IsNullOrWhiteSpace())
                        {
                            throw new UserFriendlyException("TencentCosStorageProvider BucketName is null!");
                        }
                        if (tencentCosConfig.Region.IsNullOrWhiteSpace())
                        {
                            throw new UserFriendlyException("TencentCosStorageProvider Region is null!");
                        }
                        if (tencentCosConfig.SecretId.IsNullOrWhiteSpace())
                        {
                            throw new UserFriendlyException("TencentCosStorageProvider SecretId is null!");
                        }
                        if (tencentCosConfig.SecretKey.IsNullOrWhiteSpace())
                        {
                            throw new UserFriendlyException("TencentCosStorageProvider SecretKey is null!");
                        }
                        StorageProvider = new TencentStorageProvider(tencentCosConfig);
                        break;
                    }
                default:
                    break;
            }
            #endregion
        }
    }
}
