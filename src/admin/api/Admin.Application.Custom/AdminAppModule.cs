using System.Reflection;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Modules;
using Admin.Application.Custom.AutoMapper;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Core.Custom;
using Magicodes.Admin.Core.Custom.Authorization;
using Magicodes.ExporterAndImporter.Core;

namespace Admin.Application.Custom
{
    [DependsOn(
       typeof(AppCoreModule)
       )]
    public class AdminAppModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Adding authorization providers
            Configuration.Authorization.Providers.Add<AppCustomAuthorizationProvider>();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(AdminAppDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            IocManager.Register<IExporter, Magicodes.ExporterAndImporter.Excel.ExcelExporter>(DependencyLifeStyle.Transient);
        }
    }
}
