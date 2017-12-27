using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Magicodes.Admin
{
    public class AdminClientModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AdminClientModule).GetAssembly());
        }
    }
}
