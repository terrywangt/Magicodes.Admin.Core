using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Magicodes.Admin
{
    [DependsOn(typeof(AdminXamarinSharedModule))]
    public class AdminXamarinAndroidModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AdminXamarinAndroidModule).GetAssembly());
        }
    }
}