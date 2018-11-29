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
    /// 栏目详情接口 输入参数
    /// </summary>
    public class GetColumnDetailInfoInput    
    {

		/// <summary>
		/// 栏目Id
		/// </summary>
		[Required]
		public long Id { get; set; }


    }
}