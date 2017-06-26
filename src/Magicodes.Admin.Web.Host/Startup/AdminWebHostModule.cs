using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading.BackgroundWorkers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.MultiTenancy;

namespace Magicodes.Admin.Web.Startup
{
    [DependsOn(
        typeof(AdminWebCoreModule))]
    public class AdminWebHostModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public AdminWebHostModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.Modules.AbpWebCommon().MultiTenancy.DomainFormat = _appConfiguration["App:ServerRootAddress"] ?? "http://localhost:22742/";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AdminWebHostModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            if (IocManager.Resolve<IMultiTenancyConfig>().IsEnabled)
            {
                var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
                workManager.Add(IocManager.Resolve<SubscriptionExpirationCheckWorker>());
            }
        }
    }
}
