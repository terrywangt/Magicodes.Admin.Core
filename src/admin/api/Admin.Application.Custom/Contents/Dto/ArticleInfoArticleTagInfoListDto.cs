using System;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Core.Custom.Contents;

namespace Admin.Application.Custom.Contents.Dto
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