using System;
using System.Text;
using Abp.AspNetCore.Configuration;
using Abp.AspNetZeroCore;
using Abp.AspNetZeroCore.Web;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.Configuration;
using App.Host.Authentication.JwtBearer;
using App.Host.Authentication.TwoFactor;
using App.Host.Configuration;
using Magicodes.Admin;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.EntityFrameworkCore;
using Magicodes.Admin.Web;
using Magicodes.App.Application;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

//using Magicodes.Admin.App;

//using Magicodes.App.Application;

namespace App.Host.Startup
{
    [DependsOn(
        typeof(AdminEntityFrameworkCoreModule),
        typeof(AbpAspNetZeroCoreWebModule),
        typeof(AdminCoreModule), 
        typeof(AppApplicationModule)
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

            if (_appConfiguration["Authentication:JwtBearer:IsEnabled"] != null && bool.Parse(_appConfiguration["Authentication:JwtBearer:IsEnabled"]))
            {
                ConfigureTokenAuth();
            }

            Configuration.ReplaceService<IAppConfigurationAccessor, AppConfigurationAccessor>();
        }

        private void ConfigureTokenAuth()
        {
            IocManager.Register<TokenAuthConfiguration>();
            var tokenAuthConfig = IocManager.Resolve<TokenAuthConfiguration>();

            tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"]));
            tokenAuthConfig.Issuer = _appConfiguration["Authentication:JwtBearer:Issuer"];
            tokenAuthConfig.Audience = _appConfiguration["Authentication:JwtBearer:Audience"];
            tokenAuthConfig.SigningCredentials = new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
            //90天过期
            tokenAuthConfig.Expiration = TimeSpan.FromDays(90);
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
