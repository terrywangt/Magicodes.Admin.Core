using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Configuration.Storage.Dto
{
    /// <summary>
    /// 阿里云存储配置
    /// </summary>
    public class AliStorageSettingEditDto
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// accessKeyId
        /// </summary>
        public string AccessKeyId { get; set; }
        /// <summary>
        /// accessKeySecret
        /// </summary>
        public string AccessKeySecret { get; set; }
        /// <summary>
        /// 地域节点
        /// </summary>
        public string EndPoint { get; set; }
        /// <summary>
        /// 存储桶名称
        /// </summary>
        public string BucketName { get; set; }
    }
}
