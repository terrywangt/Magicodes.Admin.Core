using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;
using Magicodes.Admin.Authorization;

namespace Magicodes.Admin
{
    /// <summary>
    /// Application layer module of the application.
    /// </summary>
    [DependsOn(
        typeof(AdminCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class AdminApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            //添加权限程序
            Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();

            //添加自定义 AutoMapper 配置
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}