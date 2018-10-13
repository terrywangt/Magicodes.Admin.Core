using System.ComponentModel.DataAnnotations;
using Magicodes.Admin.Core.Custom.LogInfos;

namespace Magicodes.App.Application.Payments.Dto
{
    /// <summary>
    ///     订单支付
    /// </summary>
    public class CreateOrderPaymentInput
    {
        /// <summary>
        ///     订单编号
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string OrderCode { get; set; }

        /// <summary>
        ///     支付渠道
        /// </summary>
        public PayChannels PayChannel { get; set; }
    }
}