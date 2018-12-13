using System.ComponentModel.DataAnnotations;
using Magicodes.Admin.LogInfos;

namespace Magicodes.App.Application.Payments.Dto
{
    public class RechargeInput
    {
        /// <summary>
        /// 充值金额
        /// </summary>
        [Required]
        [Range(0.01, 1000000)]
        public decimal TotalAmount { get; set; }
        /// <summary>
        ///     支付渠道
        /// </summary>
        public PayChannels PayChannel { get; set; }
    }
}