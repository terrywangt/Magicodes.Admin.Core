using Abp.AspNetCore.SignalR;
using Abp.AspNetZeroCore.Web;
using Abp.Hangfire;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Caching.Redis;
using Abp.Zero.Configuration;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.EntityFrameworkCore;
using Magicodes.Admin.Web.Authentication.JwtBearer;
using Magicodes.Admin.Web.Authentication.TwoFactor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Magicodes.Admin.Web
{
    [DependsOn(
        typeof(AdminEntityFrameworkCoreModule),
        typeof(AbpAspNetZeroCoreWebModule),
        typeof(AbpAspNetCoreSignalRModule),
        typeof(AbpRedisCacheModule), //AbpRedisCacheModule dependency (and Abp.RedisCache nuget package) can be removed if not using Redis cache
        typeof(AbpHangfireAspNetCoreModule) //AbpHangfireModule dependency (and Abp.Hangfire.AspNetCore nuget package) can be removed if not using Hangfire
    )]
    public class AdminWebCoreModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public AdminWebCoreModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            //Set default connection string
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                AdminConsts.ConnectionStringName
            );

            //Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            Configuration.Caching.Configure(TwoFactorCodeCacheItem.CacheName, cache =>
            {
                cache.DefaultAbsoluteExpireTime = TimeSpan.FromMinutes(2);
            });

            if (_appConfiguration["Authentication:JwtBearer:IsEnabled"] != null && bool.Parse(_appConfiguration["Authentication:JwtBearer:IsEnabled"]))
            {
                ConfigureTokenAuth();
            }

            //Uncomment this line to use Hangfire instead of default background job manager (remember also to uncomment related lines in Startup.cs file(s)).
            //Configuration.BackgroundJobs.UseHangfire();

            //使用Redis缓存替换默认的内存缓存
            if (_appConfiguration["Abp:RedisCache:IsEnabled"] == "true")
            {
                
                Configuration.Caching.UseRedis(options =>
                {
                    options.ConnectionString = _appConfiguration["Abp:RedisCache:ConnectionString"];
                    options.DatabaseId = _appConfiguration.GetValue<int>("Abp:RedisCache:DatabaseId");
                });
            }

            SetLocalizationFromWebRootXml(Path.Combine(_env.WebRootPath, "Localization"));
        }

        private void ConfigureTokenAuth()
        {
            IocManager.Register<TokenAuthConfiguration>();
            var tokenAuthConfig = IocManager.Resolve<TokenAuthConfiguration>();

            tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"]));
            tokenAuthConfig.Issuer = _appConfiguration["Authentication:JwtBearer:Issuer"];
            tokenAuthConfig.Audience = _appConfiguration["Authentication:JwtBearer:Audience"];
            tokenAuthConfig.SigningCredentials = new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
            tokenAuthConfig.Expiration = TimeSpan.FromDays(1);
        }

        public override void Initialize() => IocManager.RegisterAssemblyByConvention(typeof(AdminWebCoreModule).GetAssembly());

        public override void PostInitialize() => SetAppFolders();

        private void SetAppFolders()
        {
            var appFolders = IocManager.Resolve<AppFolders>();

            appFolders.SampleProfileImagesFolder = Path.Combine(_env.WebRootPath, $"Common{Path.DirectorySeparatorChar}Images{Path.DirectorySeparatorChar}SampleProfilePics");

            var webLogsPath = Path.Combine(_env.ContentRootPath, $"App_Data{Path.DirectorySeparatorChar}Logs");
            Directory.CreateDirectory(webLogsPath);
            appFolders.WebLogsFolder = webLogsPath;

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

            var adminPath = Path.Combine(localizationFolder, AdminConsts.LocalizationSourceName);

            if (!Directory.Exists(adminPath))
            {
                return;
            }
            
            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    AdminConsts.LocalizationSourceName,
                    new XmlFileLocalizationDictionaryProvider(
                        adminPath
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
            //TODO:AbpZero,App

        }
    }
}
