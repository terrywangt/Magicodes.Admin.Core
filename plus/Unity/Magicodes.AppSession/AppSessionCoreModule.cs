using System.Reflection;
using Abp.AspNetCore;
using Abp.Modules;

namespace Magicodes.AppSession
{
    [DependsOn(
        typeof(AbpAspNetCoreModule)
    )]
    public class AppSessionCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
        }
    }
}