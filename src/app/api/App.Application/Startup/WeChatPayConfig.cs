using Magicodes.Pay.WeChat.Config;

namespace Magicodes.App.Application.Startup
{
    public class WeChatPayConfig : IWeChatPayConfig
    {
        public string PayAppId { get; set; }
        public string MchId { get; set; }
        public string PayNotifyUrl { get; set; }
        public string TenPayKey { get; set; }
    }
}