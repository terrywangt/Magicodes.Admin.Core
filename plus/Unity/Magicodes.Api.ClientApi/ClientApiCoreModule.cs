using System.Reflection;
using Abp.AspNetCore;
using Abp.Modules;

namespace Magicodes.Api.ClientApi
{
    [DependsOn(
        typeof(AbpAspNetCoreModule)
    )]
    public class ClientApiCoreModule : AbpModule
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