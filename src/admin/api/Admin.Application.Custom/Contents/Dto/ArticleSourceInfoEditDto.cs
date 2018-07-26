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
    ///  文章来源编辑Dto
    /// </summary>
    [AutoMapFrom(typeof(ArticleSourceInfo))]
    public class ArticleSourceInfoEditDto : EntityDto<long?>
    {
		/// <summary>
		/// 名称
		/// </summary>
		[Required][MaxLength(30)]
        public string Name { get; set; }
    }
}