using Magicodes.Admin.Configuration.Host.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Configuration.Pay.Dto
{
    public class PaySettingEditDto
    {
        /// <summary>
        /// 微信支付
        /// </summary>
        public WeChatPaySettingEditDto WeChatPay { get; set; }


        /// <summary>
        /// 阿里支付
        /// </summary>
        public AliPaySettingEditDto AliPay { get; set; }
    }
}
