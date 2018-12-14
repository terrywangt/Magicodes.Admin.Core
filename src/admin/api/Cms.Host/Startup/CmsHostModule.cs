

using System;
using System.Collections.Generic;
using Abp.AspNetZeroCore;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading.BackgroundWorkers;
using Abp.Zero.Configuration;
using Cms.Host.Configuration;
using Magicodes.Admin;
using Magicodes.Admin.Chat;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.Contents;
using Magicodes.Admin.EntityFrameworkCore;
using Magicodes.Admin.Friendships;
using Magicodes.Admin.MultiTenancy;
using Magicodes.Unity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Runtime.Caching.Redis;

namespace Cms.Host.Startup
{
    [DependsOn(
        typeof(AdminApplicationModule),
        typeof(UnityModule)
    )]
    public class CmsHostModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public CmsHostModule(
            IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.Modules.AspNetZero().LicenseCode = _appConfiguration["AbpZeroLicenseCode"];
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                AdminConsts.ConnectionStringName
            );
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            Configuration.ReplaceService<IAppConfigurationAccessor, AppConfigurationAccessor>();

            //使用Redis缓存替换默认的内存缓存
            if (_appConfiguration["Abp:RedisCache:ConnectionString:IsEnabled"] == "true")
            {

                Configuration.Caching.UseRedis(options =>
                {
                    options.ConnectionString = _appConfiguration["Abp:RedisCache:ConnectionString"];
                    options.DatabaseId = _appConfiguration.GetValue<int>("Abp:RedisCache:DatabaseId");
                });
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CmsHostModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<IColumnInfoAppService>();
        }
    }
}
