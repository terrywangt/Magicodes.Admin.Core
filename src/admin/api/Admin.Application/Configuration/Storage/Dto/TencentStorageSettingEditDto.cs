using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.Admin.Configuration.Storage.Dto
{
    public class TencentStorageSettingEditDto
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        ///     应用ID。
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        ///     秘钥id
        /// </summary>
        public string SecretId { get; set; }

        /// <summary>
        ///     秘钥Key
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        ///     区域
        /// </summary>
        public string Region { get; set; } = "ap-guangzhou";

        /// <summary>
        ///    存储桶名称
        /// </summary>
        public string BucketName { get; set; }
    }
}
