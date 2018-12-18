using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Configuration.Storage.Dto
{

    public class StorageSettingEditDto
    {
        /// <summary>
        /// 阿里云存储
        /// </summary>
        public AliStorageSettingEditDto AliStorageSetting { get; set; }

        /// <summary>
        /// 腾讯云存储
        /// </summary>
        public TencentStorageSettingEditDto TencentStorageSetting { get; set; }
    }
}
