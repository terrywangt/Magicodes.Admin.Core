using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetZeroCore;
using Abp.AspNetZeroCore.Web;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Caching.Redis;
using Abp.Zero.Configuration;
using Cms.Host.Configuration;
using Magicodes.Admin;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.Core.Custom;
using Magicodes.Admin.EntityFrameworkCore;
using Magicodes.Unity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Cms.Host.Startup
{
    [DependsOn(
        typeof(AdminEntityFrameworkCoreModule),
        typeof(AbpAspNetZeroCoreWebModule),
        typeof(AdminCoreModule),
        typeof(AppCoreModule),
        typeof(AdminApplicationModule),
        //typeof(AbpAspNetCoreModule),
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
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CmsHostModule).GetAssembly());
        }

        public override void PostInitialize()
        {
        }
    }
}