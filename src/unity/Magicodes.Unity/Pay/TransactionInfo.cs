using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Magicodes.Admin.Core.Custom.LogInfos;

namespace Magicodes.Unity.Pay
{
    public class TransactionInfo
    {
        /// <summary>
        /// 金额
        /// </summary>
        [Display(Name = "金额")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 支付渠道
        /// </summary>
        [Display(Name = "支付渠道")]
        public PayChannels PayChannel { get; set; }

        /// <summary>
        /// 交易状态
        /// </summary>
        [Display(Name = "交易状态")]
        public TransactionStates TransactionState { get; set; }

        /// <summary>
        /// 自定义数据
        /// </summary>
        [MaxLength(500)]
        [Display(Name = "自定义数据")]
        public string CustomData { get; set; }

        /// <summary>
        /// 交易单号
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "交易单号")]
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 支付订单号（比如微信支付订单号）
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "支付订单号")]
        public string TransactionId { get; set; }

        /// <summary>
        /// 支付完成时间
        /// </summary>
        [Display(Name = "支付完成时间")]
        public DateTime? PayTime { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public  Exception Exception { get; set; }
    }
}
