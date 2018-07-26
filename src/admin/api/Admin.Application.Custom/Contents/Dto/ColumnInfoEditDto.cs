using System;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using Admin.Application.Custom.Contents;
using Magicodes.Admin.Core.Custom.Contents;

namespace Admin.Application.Custom.Contents.Dto
{
    /// <summary>
    ///  栏目编辑Dto
    /// </summary>
    [AutoMapFrom(typeof(ColumnInfo))]
    public class ColumnInfoEditDto : EntityDto<long?>
    {
		/// <summary>
		/// 标题
		/// </summary>
		[Required][MaxLength(50)]
        public string Title { get; set; }
		/// <summary>
		/// 排序号
		/// </summary>
		
        public long? SortNo { get; set; }
		/// <summary>
		/// 是否启用
		/// </summary>
		
        public bool IsActive { get; set; }
		/// <summary>
		/// 授权访问
		/// </summary>
		
        public bool IsNeedAuthorizeAccess { get; set; }
		/// <summary>
		/// 描述
		/// </summary>
		[MaxLength(500)]
        public string Description { get; set; }
		/// <summary>
		/// 简介
		/// </summary>
		[MaxLength(200)]
        public string Introduction { get; set; }
		/// <summary>
		/// 小图标
		/// </summary>
		[MaxLength(20)]
        public string IconCls { get; set; }
		/// <summary>
		/// 链接
		/// </summary>
		[MaxLength(255)]
        public string Url { get; set; }
    }
}