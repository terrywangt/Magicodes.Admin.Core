using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Modules;
using Magicodes.Admin.Authorization;
using Magicodes.App.Core;
using Magicodes.ExporterAndImporter.Core;
using System;
using System.Reflection;

namespace Magicodes.Admin.Application.App
{
    [DependsOn(
       typeof(AppCoreModule),
       typeof(AbpAutoMapperModule)
       )]
    public class AdminAppModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpAutoMapper();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            IocManager.Register<IExporter, ExporterAndImporter.Excel.ExcelExporter>(DependencyLifeStyle.Transient);
        }
    }
}
