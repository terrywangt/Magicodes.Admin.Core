using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Magicodes.Admin.Contents.Dto
{
    /// <summary>
    ///  列表Dto
    /// </summary>
    [AutoMapFrom(typeof(ArticleTagInfo))]
    public partial class ArticleTagInfoListDto : EntityDto<long>
    {
		/// <summary>
		/// 
        /// <code>
        /// 请配置AutoMap：
        /// .ForMember(dto => dto.ArticleInfo, options => options.MapFrom(p => p.ArticleInfo.Title))
        /// </code>
        /// </summary>
        public string ArticleInfo { get; set; }
		/// <summary>
		/// 名称
		/// </summary>
        public string Name { get; set; }
		/// <summary>
		/// 创建时间
		/// </summary>
        public DateTime CreationTime { get; set; }


        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}