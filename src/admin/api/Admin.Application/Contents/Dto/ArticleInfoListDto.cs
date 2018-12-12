using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Magicodes.Admin.Contents.Dto
{
    /// <summary>
    ///  文章列表Dto
    /// </summary>
    [AutoMapFrom(typeof(ArticleInfo))]
    public partial class ArticleInfoListDto : EntityDto<long>
    {
        /// <summary>
        ///     编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
		/// <summary>
		/// 发布人（机构）
		/// </summary>
        public string Publisher { get; set; }
		/// <summary>
		/// 栏目信息
        /// <code>
        /// 请配置AutoMap：
        /// .ForMember(dto => dto.ColumnInfo, options => options.MapFrom(p => p.ColumnInfo.Title))
        /// </code>
        /// </summary>
        public string ColumnInfo { get; set; }
		/// <summary>
		/// 文章来源
        /// <code>
        /// 请配置AutoMap：
        /// .ForMember(dto => dto.ArticleSourceInfo, options => options.MapFrom(p => p.ArticleSourceInfo.Name))
        /// </code>
        /// </summary>
        public string ArticleSourceInfo { get; set; }
		/// <summary>
		/// 发布时间
		/// </summary>
        public DateTime? ReleaseTime { get; set; }
		/// <summary>
		/// 是否启用
		/// </summary>
        public bool IsActive { get; set; }
		/// <summary>
		/// 授权访问
		/// </summary>
        public bool IsNeedAuthorizeAccess { get; set; }
		/// <summary>
		/// 标题
		/// </summary>
        public string SeoTitle { get; set; }
		/// <summary>
		/// 静态页路径
		/// </summary>
        public string StaticPageUrl { get; set; }
		/// <summary>
		/// 图片URL
		/// </summary>
        public string ImageUrl { get; set; }

		/// <summary>
		/// 链接
		/// </summary>
        public string Url { get; set; }
		/// <summary>
		/// 推荐类型
		/// </summary>
        public RecommendedTypes RecommendedType { get; set; }
		/// <summary>
		/// 访问数
		/// </summary>
        public long ViewCount { get; set; }
		/// <summary>
		/// 创建时间
		/// </summary>
        public DateTime CreationTime { get; set; }


        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        ///     是否静态
        /// </summary>
        public bool IsStatic { get; set; }
    }
}