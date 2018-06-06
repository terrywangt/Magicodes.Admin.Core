using Abp.AspNetZeroCore;
using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero;
using Magicodes.Admin;

namespace Magicodes.App.Core
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