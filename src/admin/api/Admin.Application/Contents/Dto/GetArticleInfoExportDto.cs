using System;
using Abp.AutoMapper;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Excel;

namespace Magicodes.Admin.Contents.Dto
{
    /// <summary>
    ///  文章导出Dto
    /// </summary>
	[ExcelExporter(Name = "文章", TableStyle = "Light10")]
    [AutoMapFrom(typeof(ArticleInfo))]
    public class ArticleInfoExportDto
    {
        /// <summary>
        ///     编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [ExporterHeader(DisplayName = "标题", IsAutoFit = true)]
        public string Title { get; set; }

		/// <summary>
		/// 发布人（机构）
		/// </summary>
		[ExporterHeader(DisplayName = "发布人（机构）", IsAutoFit = true)]
        public string Publisher { get; set; }

		/// <summary>
		/// 栏目信息
		/// </summary>
		[ExporterHeader(DisplayName = "栏目信息", IsAutoFit = true)]
        public ColumnInfo ColumnInfo { get; set; }

		/// <summary>
		/// 文章来源
		/// </summary>
		[ExporterHeader(DisplayName = "文章来源", IsAutoFit = true)]
        public ArticleSourceInfo ArticleSourceInfo { get; set; }

		/// <summary>
		/// 发布时间
		/// </summary>
		[ExporterHeader(Format="yyyy-MM-dd HH:mm:ss")]
        public DateTime? ReleaseTime { get; set; }

		/// <summary>
		/// 标题
		/// </summary>
		[ExporterHeader(DisplayName = "标题", IsAutoFit = true)]
        public string SeoTitle { get; set; }

		/// <summary>
		/// 静态页路径
		/// </summary>
		[ExporterHeader(DisplayName = "静态页路径", IsAutoFit = true)]
        public string StaticPageUrl { get; set; }

		/// <summary>
		/// 链接
		/// </summary>
		[ExporterHeader(DisplayName = "链接", IsAutoFit = true)]
        public string Url { get; set; }

		/// <summary>
		/// 推荐类型
		/// </summary>
		[ExporterHeader(DisplayName = "推荐类型", IsAutoFit = true)]
        public RecommendedTypes RecommendedType { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[ExporterHeader(Format="yyyy-MM-dd HH:mm:ss")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        ///     是否静态
        /// </summary>
        public bool IsStatic { get; set; }

    }
}