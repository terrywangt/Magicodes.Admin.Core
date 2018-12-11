using System;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Magicodes.Admin.Dto;
using Abp.Runtime.Validation;
using Abp.Extensions;

namespace Magicodes.App.Application.Contents.Dto
{
    /// <summary>
    /// 获取轮询图 输入参数
    /// </summary>
    public class GetCarouselPictureListInput    
    {

		/// <summary>
		/// 轮询图位置
		/// </summary>
		[Required]
		public PositionEnum Position { get; set; }


        public enum PositionEnum
        {
            /// <summary>
            /// 首页（默认）
            /// </summary>
            Default = 0, 

        }

    }
}