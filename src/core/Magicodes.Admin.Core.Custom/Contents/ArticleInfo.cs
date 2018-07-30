using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;

namespace Magicodes.Admin.Core.Custom.Contents
{
    /// <summary>
    /// 文章
    /// </summary>
    [Display(Name = "文章", GroupName = "文章基础信息")]
    public class ArticleInfo : EntityBase<long>, IPassivable
    {
        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        /// <summary>
        /// 发布人（机构）
        /// </summary>
        [Display(Name = "发布人（机构）")]
        [Required]
        [MaxLength(20)]
        public string Publisher { get; set; }

        public long ColumnInfoId { get; set; }

        /// <summary>
        /// 栏目信息
        /// </summary>
        [Display(Name = "栏目信息")]
        public virtual ColumnInfo ColumnInfo { get; set; }

        public long? ArticleSourceInfoId { get; set; }

        /// <summary>
        /// 文章来源
        /// </summary>
        [Display(Name = "文章来源")]
        public virtual ArticleSourceInfo ArticleSourceInfo { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        [Display(Name = "发布时间")]
        public DateTime? ReleaseTime { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        [Required]
        [DataType(DataType.Html)]
        public string Content { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [Display(Name = "是否启用")]
        [DefaultValue(true)]
        public bool IsActive { get; set; }

        /// <summary>
        /// 授权访问
        /// </summary>
        [Display(Name = "授权访问")]
        public bool IsNeedAuthorizeAccess { get; set; }

        #region SEO
        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题", GroupName = "SEO")]
        [MaxLength(50)]
        public string SeoTitle { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        [Display(Name = "关键字（多个以逗号隔开）", GroupName = "SEO")]
        [MaxLength(200)]
        [DataType(DataType.MultilineText)]
        public string KeyWords { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        [Display(Name = "简介", GroupName = "SEO")]
        [MaxLength(200)]
        [DataType(DataType.MultilineText)]
        public string Introduction { get; set; }

        /// <summary>
        /// 静态页路径
        /// </summary>
        [Display(Name = "静态页路径", GroupName = "SEO")]
        [MaxLength(200)]
        public string StaticPageUrl { get; set; }
        #endregion

        /// <summary>
        /// 封面
        /// </summary>
        [DataType(DataType.ImageUrl)]
        [Display(Name = "封面")]
        public long? Cover { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        [Display(Name = "标签")]
        public virtual ICollection<ArticleTagInfo> ArticleTagInfos { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        [Display(Name = "链接", GroupName = "扩展信息")]
        [DataType(DataType.Url)]
        [MaxLength(255)]
        public string Url { get; set; }

        /// <summary>
        /// 推荐类型
        /// </summary>
        [Display(Name = "推荐类型")]
        public RecommendedTypes RecommendedType { get; set; }

        /// <summary>
        /// 访问数
        /// </summary>
        [Display(Name = "访问数", Prompt = "ignore[form]")]
        public long ViewCount { get; set; }
    }
}