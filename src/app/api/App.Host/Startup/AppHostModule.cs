using System;
using System.Text;
using Abp.AspNetCore.Configuration;
using Abp.AspNetZeroCore;
using Abp.AspNetZeroCore.Web;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.Configuration;
using App.Host.Authentication.TwoFactor;
using App.Host.Configuration;
using Magicodes.Admin;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.Core.Custom;
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
        typeof(AppCoreModule),
        typeof(AppApplicationModule),
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
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AppHostModule).GetAssembly());
        }

        public override void PostInitialize()
        {

        }
    }
}
