using System;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Runtime.Validation;
using Abp.Extensions;

namespace Magicodes.App.Application.Contents.Dto
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
		public long Id { get; set; }
        /// <summary>
        /// 文章编号
        /// </summary>
        public string Code { get; set; }


    }
}