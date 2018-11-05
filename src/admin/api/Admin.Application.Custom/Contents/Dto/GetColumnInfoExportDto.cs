using System;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Magicodes.ExporterAndImporter.Excel;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.Admin.Core.Custom.Contents;

namespace Admin.Application.Custom.Contents.Dto
{
    /// <summary>
    ///  栏目导出Dto
    /// </summary>
	[ExcelExporter(Name = "栏目", TableStyle = "Light10")]
    [AutoMapFrom(typeof(ColumnInfo))]
    public class ColumnInfoExportDto
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
		/// 小图标
		/// </summary>
		[ExporterHeader(DisplayName = "小图标", IsAutoFit = true)]
        public string IconCls { get; set; }

		/// <summary>
		/// 链接
		/// </summary>
		[ExporterHeader(DisplayName = "链接", IsAutoFit = true)]
        public string Url { get; set; }

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