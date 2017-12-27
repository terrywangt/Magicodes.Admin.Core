using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Magicodes.Admin
{
    [DependsOn(typeof(AdminXamarinSharedModule))]
    public class AdminXamarinIosModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AdminXamarinIosModule).GetAssembly());
        }
    }
}