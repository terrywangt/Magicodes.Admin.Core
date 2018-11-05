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
    ///  文章编辑Dto
    /// </summary>
    [AutoMapFrom(typeof(ArticleInfo))]
    public class ArticleInfoEditDto : EntityDto<long?>
    {
        /// <summary>
        /// 标题
        /// </summary>
        [Required][MaxLength(50)]
        public string Title { get; set; }
		/// <summary>
		/// 发布人（机构）
		/// </summary>
		[Required][MaxLength(20)]
        public string Publisher { get; set; }
        public long ColumnInfoId { get; set; }
        public long? ArticleSourceInfoId { get; set; }
		/// <summary>
		/// 发布时间
		/// </summary>
		
        public DateTime? ReleaseTime { get; set; }
		/// <summary>
		/// 内容
		/// </summary>
		[Required]
        public string Content { get; set; }
		/// <summary>
		/// 是否启用
		/// </summary>
		
        public bool IsActive { get; set; } = true ;
		/// <summary>
		/// 授权访问
		/// </summary>
		
        public bool IsNeedAuthorizeAccess { get; set; }
		/// <summary>
		/// 标题
		/// </summary>
		[MaxLength(50)]
        public string SeoTitle { get; set; }
		/// <summary>
		/// 关键字（多个以逗号隔开）
		/// </summary>
		[MaxLength(200)]
        public string KeyWords { get; set; }
		/// <summary>
		/// 简介
		/// </summary>
		[MaxLength(200)]
        public string Introduction { get; set; }
		/// <summary>
		/// 静态页路径
		/// </summary>
		[MaxLength(200)]
        public string StaticPageUrl { get; set; }
		/// <summary>
		/// 链接
		/// </summary>
		[MaxLength(255)]
        public string Url { get; set; }
		/// <summary>
		/// 推荐类型
		/// </summary>
		[Required]
        public RecommendedTypes RecommendedType { get; set; }

        /// <summary>
        ///     是否静态
        /// </summary>
        public bool IsStatic { get; set; }
    }
}