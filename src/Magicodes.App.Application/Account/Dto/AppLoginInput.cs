using System;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Magicodes.Admin.Dto;
using Abp.Runtime.Validation;
using Abp.Extensions;

namespace Magicodes.App.Application.Account.Dto
{
    /// <summary>
    /// 登陆 输入参数
    /// </summary>
    public class AppLoginInput    
    {

		/// <summary>
		/// 手机号码
		/// </summary>
		[Required]
		public string Phone { get; set; }

		/// <summary>
		/// 验证码
		/// </summary>
		[Required]
		public string Code { get; set; }


    }
}