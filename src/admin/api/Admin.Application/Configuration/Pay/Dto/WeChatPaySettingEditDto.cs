using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Configuration.Pay.Dto
{
    public class WeChatPaySettingEditDto
    {
        /// <summary>
        /// 支付平台分配给开发者的应用ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 商户Id
        /// </summary>
        public string MchId { get; set; }

        /// <summary>
        /// 支付回调路径
        /// </summary>
        public string PayNotifyUrl { get; set; }

        /// <summary>
        /// 支付密钥
        /// </summary>
        public string TenPayKey { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }
    }
}
