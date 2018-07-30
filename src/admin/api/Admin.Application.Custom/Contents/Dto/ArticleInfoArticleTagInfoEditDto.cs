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
    ///  编辑Dto
    /// </summary>
    [AutoMapFrom(typeof(ArticleTagInfo))]
    public class ArticleTagInfoEditDto : EntityDto<long?>
    {
        public long ArticleInfoId { get; set; }
		/// <summary>
		/// 名称
		/// </summary>
		[Required][MaxLength(50)]
        public string Name { get; set; }
    }
}