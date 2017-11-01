using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Magicodes.Admin.Authorization;

namespace Magicodes.Admin
{
    /// <summary>
    /// Application layer module of the application.
    /// </summary>
    [DependsOn(
        typeof(AdminCoreModule)
        )]
    public class AdminApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Adding authorization providers
            Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AdminApplicationModule).GetAssembly());
        }
    }
}