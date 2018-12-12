using Magicodes.Admin.LogInfos;

namespace Magicodes.Pay.Services.Dto
{
    public class PayInput : AppPayInput
    {
        /// <summary>
        /// 支付渠道
        /// </summary>
        public PayChannels PayChannel { get; set; }
    }
}
