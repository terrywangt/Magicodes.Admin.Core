using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Configuration.Pay.Dto
{
    public class GlobalAlipaySettingEditDto
    {
        public GlobalAlipaySettingEditDto()
        {
            SplitFundSettings = new List<SplitFundSettingDto>();
        }

        /// <summary>
        ///     Gets or sets the Key
        ///     MD5密钥，安全检验码，由数字和字母组成的32位字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Gets or sets the Partner
        ///     合作商户uid(合作身份者ID，签约账号，以2088开头由16位纯数字组成的字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm)
        /// </summary>
        public string Partner { get; set; }

        /// <summary>
        ///     Gets or sets the Gatewayurl
        ///     支付宝网关
        /// </summary>
        public string Gatewayurl { get; set; } = "https://mapi.alipay.com/gateway.do";

        /// <summary>
        ///     Gets or sets the Notify
        ///     服务器异步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数,必须外网可以正常访问
        /// </summary>
        public string Notify { get; set; }

        /// <summary>
        ///     页面跳转同步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数，必须外网可以正常访问
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     结算币种
        /// </summary>
        public string Currency { get; set; } = "USD";

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 分账信息
        /// </summary>
        public List<SplitFundSettingDto> SplitFundSettings { get; set; }
    }
}
