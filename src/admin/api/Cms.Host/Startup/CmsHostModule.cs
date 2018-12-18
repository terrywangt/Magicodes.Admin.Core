using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetZeroCore;
using Abp.AspNetZeroCore.Web;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Caching.Redis;
using Abp.Zero.Configuration;
using Cms.Host.Configuration;
using Magicodes.Admin;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.EntityFrameworkCore;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.Unity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Cms.Host.Startup
{
    [DependsOn(
        typeof(AdminEntityFrameworkCoreModule),
        typeof(AbpAspNetZeroCoreWebModule),
        typeof(AdminCoreModule),
        typeof(AdminApplicationModule),
        typeof(AbpRedisCacheModule),
        typeof(UnityModule)
    )]
    public class CmsHostModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IHostingEnvironment _env;


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

            //Use database for language management
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
            IocManager.Register<IExporter, Magicodes.ExporterAndImporter.Excel.ExcelExporter>(DependencyLifeStyle.Transient);

        }

        public override void PostInitialize()
        {
        }
    }
}