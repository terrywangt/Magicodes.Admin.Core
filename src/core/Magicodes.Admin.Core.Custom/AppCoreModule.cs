using Abp.AspNetZeroCore;
using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero;

namespace Magicodes.Admin.Core.Custom
{
    [DependsOn(
        typeof(AbpZeroCoreModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpAspNetZeroCoreModule),
        typeof(AdminCoreModule))]
    public class AppCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AppCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
           
        }
    }
}