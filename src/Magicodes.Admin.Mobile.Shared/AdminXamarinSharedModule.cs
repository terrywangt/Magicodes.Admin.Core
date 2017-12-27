using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Magicodes.Admin
{
    [DependsOn(typeof(AdminClientModule), typeof(AbpAutoMapperModule))]
    public class AdminXamarinSharedModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.IsEnabled = false;
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AdminXamarinSharedModule).GetAssembly());
        }
    }
}