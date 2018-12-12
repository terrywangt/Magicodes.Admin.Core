using System;
using Abp.AutoMapper;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Excel;

namespace Magicodes.Admin.Contents.Dto
{
    /// <summary>
    ///  导出Dto
    /// </summary>
	[ExcelExporter(Name = "", TableStyle = "Light10")]
    [AutoMapFrom(typeof(ArticleTagInfo))]
    public class ArticleTagInfoExportDto
    {
		/// <summary>
		/// 
		/// </summary>
		[ExporterHeader(DisplayName = "", IsAutoFit = true)]
        public ArticleInfo ArticleInfo { get; set; }

		/// <summary>
		/// 名称
		/// </summary>
		[ExporterHeader(DisplayName = "名称", IsAutoFit = true)]
        public string Name { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[ExporterHeader(Format="yyyy-MM-dd HH:mm:ss")]
        public DateTime CreationTime { get; set; }

    }
}