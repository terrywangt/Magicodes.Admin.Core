using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Magicodes.Admin.Contents.Dto
{
    /// <summary>
    ///  文章来源列表Dto
    /// </summary>
    [AutoMapFrom(typeof(ArticleSourceInfo))]
    public partial class ArticleSourceInfoListDto : EntityDto<long>
    {
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