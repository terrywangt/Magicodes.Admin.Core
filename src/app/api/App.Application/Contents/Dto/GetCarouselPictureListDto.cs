using System;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.App.Application.Contents.Dto
{
    /// <summary>
    /// 获取轮询图 输出参数
    /// </summary>
    public class GetCarouselPictureListDto    
    {
        /// <summary>
        /// 图片链接
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 点击图片链接的URL
        /// </summary>
        public string Url { get; set; }


    }
}