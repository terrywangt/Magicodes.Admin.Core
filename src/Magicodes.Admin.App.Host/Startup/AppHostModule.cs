using Abp.AspNetZeroCore;
using Abp.AspNetZeroCore.Web.Authentication.External;
using Abp.AspNetZeroCore.Web.Authentication.External.Facebook;
using Abp.AspNetZeroCore.Web.Authentication.External.Google;
using Abp.AspNetZeroCore.Web.Authentication.External.Microsoft;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading.BackgroundWorkers;
using Abp.Timing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.EntityFrameworkCore;
using Magicodes.Admin.MultiTenancy;
//using Magicodes.Admin.App;
using Abp.AspNetCore.Configuration;
//using Magicodes.App.Application;

namespace Magicodes.Admin.Web.Startup
{
    [DependsOn(
        typeof(AdminWebCoreModule),
        typeof(AdminCoreModule)
    //,typeof(AppApplicationModule)
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
            Configuration.Modules.AbpWebCommon().MultiTenancy.DomainFormat = _appConfiguration["App:ServerRootAddress"] ?? "http://localhost:22742/";
            Configuration.Modules.AspNetZero().LicenseCode = _appConfiguration["AbpZeroLicenseCode"];

            ////配置App动态web api
            //Configuration.Modules.AbpAspNetCore()
            //    .CreateControllersForAppServices(
            //        typeof(AppApplicationModule).GetAssembly(), "app"
            //    );
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
