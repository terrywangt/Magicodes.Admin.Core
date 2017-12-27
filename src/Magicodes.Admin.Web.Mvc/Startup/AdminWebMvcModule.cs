using Abp.AspNetZeroCore;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading.BackgroundWorkers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.EntityFrameworkCore;
using Magicodes.Admin.MultiTenancy;
using Magicodes.Admin.Web.Areas.Admin.Startup;

namespace Magicodes.Admin.Web.Startup
{
    [DependsOn(
        typeof(AdminWebCoreModule)
    )]
    public class AdminWebMvcModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public AdminWebMvcModule(
            IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.Modules.AbpWebCommon().MultiTenancy.DomainFormat = _appConfiguration["App:WebSiteRootAddress"] ?? "http://localhost:62114/";
            Configuration.Modules.AspNetZero().LicenseCode = _appConfiguration["AbpZeroLicenseCode"];
            Configuration.Navigation.Providers.Add<AdminNavigationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AdminWebMvcModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            if (!IocManager.Resolve<IMultiTenancyConfig>().IsEnabled)
            {
                return;
            }

            if (!DatabaseCheckHelper.Exist(_appConfiguration["ConnectionStrings:Default"]))
            {
                return;
            }

            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            workManager.Add(IocManager.Resolve<SubscriptionExpirationCheckWorker>());
            workManager.Add(IocManager.Resolve<SubscriptionExpireEmailNotifierWorker>());
        }
    }
}