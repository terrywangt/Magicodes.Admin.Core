using System;
using System.IO;
using System.Linq;
using System.Text;
using Abp.AspNetCore.Configuration;
using Abp.AspNetZeroCore;
using Abp.AspNetZeroCore.Web;
using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Caching.Redis;
using Abp.Zero.Configuration;
using App.Host.Authentication.TwoFactor;
using App.Host.Configuration;
using Magicodes.Admin;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.EntityFrameworkCore;
using Magicodes.App.Application;
using Magicodes.App.Application.Configuration;
using Magicodes.Sms;
using Magicodes.Unity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace App.Host.Startup
{
    [DependsOn(
        typeof(AdminEntityFrameworkCoreModule),
        typeof(AbpAspNetZeroCoreWebModule),
        typeof(SmsModule),
        typeof(AdminCoreModule),
        typeof(AppApplicationModule),
        typeof(AbpRedisCacheModule),
        typeof(UnityModule)
    )]
    public class AppHostModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public AppHostModule(
            IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            //Configuration.Modules.AbpWebCommon().MultiTenancy.DomainFormat = _appConfiguration["App:ServerRootAddress"] ?? "http://localhost:22742/";
            Configuration.Modules.AspNetZero().LicenseCode = _appConfiguration["AbpZeroLicenseCode"];

            //配置App动态web api
            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(AppApplicationModule).GetAssembly(), "app"
                );

            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                AdminConsts.ConnectionStringName
            );

            //Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            Configuration.Caching.Configure(TwoFactorCodeCacheItem.CacheName, cache =>
            {
                cache.DefaultAbsoluteExpireTime = TimeSpan.FromMinutes(2);
            });

            Configuration.ReplaceService<IAppConfigurationAccessor, AppConfigurationAccessor>();

            //使用Redis缓存替换默认的内存缓存
            if (Convert.ToBoolean(_appConfiguration["Abp:RedisCache:IsEnabled"] ?? "false"))
            {
                Configuration.Caching.UseRedis(options =>
                {
                    options.ConnectionString = _appConfiguration["Abp:RedisCache:ConnectionString"];
                    options.DatabaseId = _appConfiguration.GetValue<int>("Abp:RedisCache:DatabaseId");
                });
            }

            SetLocalizationFromWebRootXml(Path.Combine(_env.WebRootPath, "Localization"));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AppHostModule).GetAssembly());
        }

        public override void PostInitialize()
        {

        }

        /// <summary>
        /// 从WebRoot初始化多语言
        /// </summary>
        /// <param name="localizationFolder"></param>
        private void SetLocalizationFromWebRootXml(string localizationFolder)
        {
            if (!Directory.Exists(localizationFolder))
            {
                return;
            }

            var appPath = Path.Combine(localizationFolder, AdminConsts.AppLocalizationSourceName);

            if (!Directory.Exists(appPath))
            {
                return;
            }

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    AdminConsts.AppLocalizationSourceName,
                    new XmlFileLocalizationDictionaryProvider(
                        appPath
                    )
                )
            );

            //移除Abp源,添加自己的语言定义
            Configuration.Localization.Sources.Remove(Configuration.Localization.Sources.First(p => p.Name == "Abp"));
            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    "Abp",
                    new XmlFileLocalizationDictionaryProvider(
                        Path.Combine(localizationFolder, "Abp")
                    )
                )
            );

            //移除Abp源,添加自己的语言定义
            Configuration.Localization.Sources.Remove(Configuration.Localization.Sources.First(p => p.Name == "AbpWeb"));
            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    "AbpWeb",
                    new XmlFileLocalizationDictionaryProvider(
                        Path.Combine(localizationFolder, "AbpWeb")
                    )
                )
            );
            //TODO:AbpZero

        }
    }
}
