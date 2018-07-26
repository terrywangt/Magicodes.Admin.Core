using System.Reflection;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Modules;
using Admin.Application.Custom.Contents.Dto;
using Magicodes.Admin.Core.Custom;
using Magicodes.Admin.Core.Custom.Contents;
using Magicodes.ExporterAndImporter.Core;

namespace Admin.Application.Custom
{
    [DependsOn(
       typeof(AppCoreModule),
       typeof(AbpAutoMapperModule)
       )]
    public class AdminAppModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpAutoMapper().Configurators.Add(config =>
            {
                config.CreateMap<ArticleInfo, ArticleInfoListDto>()
                    .ForMember(dto => dto.ColumnInfo, options => options.MapFrom(p => p.ColumnInfo.Title))
                    .ForMember(dto => dto.ArticleSourceInfo, options => options.MapFrom(p => p.ArticleSourceInfo.Name));

            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            IocManager.Register<IExporter, Magicodes.ExporterAndImporter.Excel.ExcelExporter>(DependencyLifeStyle.Transient);
        }
    }
}
