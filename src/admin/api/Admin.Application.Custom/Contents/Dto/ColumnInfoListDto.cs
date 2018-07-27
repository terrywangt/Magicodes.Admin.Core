using System;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Core.Custom.Contents;

namespace Admin.Application.Custom.Contents.Dto
{
    /// <summary>
    ///  栏目列表Dto
    /// </summary>
    [AutoMapFrom(typeof(ColumnInfo))]
    public partial class ColumnInfoListDto : EntityDto<long>
    {
		/// <summary>
		/// 标题
		/// </summary>
        public string Title { get; set; }
		/// <summary>
		/// 是否启用
		/// </summary>
        public bool IsActive { get; set; }
		/// <summary>
		/// 授权访问
		/// </summary>
        public bool IsNeedAuthorizeAccess { get; set; }
		/// <summary>
		/// 小图标
		/// </summary>
        public string IconCls { get; set; }
		/// <summary>
		/// 图片URL
		/// </summary>
        public string ImageUrl { get; set; }

		/// <summary>
		/// 链接
		/// </summary>
        public string Url { get; set; }
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