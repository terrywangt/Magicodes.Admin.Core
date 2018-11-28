using System;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Magicodes.Admin.Dto;
using Abp.Runtime.Validation;
using Abp.Extensions;

namespace Magicodes.App.Application.Contents.Contents.Dto
{
    /// <summary>
    /// 文章详情接口 输入参数
    /// </summary>
    public class GetArticleDetailInfoInput    
    {

		/// <summary>
		/// 文章Id
		/// </summary>
		[Required]
		public long ArticleInfoId { get; set; }


    }
}