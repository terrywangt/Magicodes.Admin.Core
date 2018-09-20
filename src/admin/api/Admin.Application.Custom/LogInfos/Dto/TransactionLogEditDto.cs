using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Magicodes.Admin.Core.Custom.LogInfos;
using System;
using System.ComponentModel.DataAnnotations;

namespace Admin.Application.Custom.LogInfos.Dto
{
    /// <summary>
    ///  交易日志编辑Dto
    /// </summary>
    [AutoMapFrom(typeof(TransactionLog))]
    public class TransactionLogEditDto : EntityDto<long?>
    {
        /// <summary>
        /// 金额
        /// </summary>
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// 客户端Ip
        /// </summary>
        [MaxLength(64)]
        public string ClientIpAddress { get; set; }
        /// <summary>
        /// 客户端名称
        /// </summary>
        [MaxLength(128)]
        public string ClientName { get; set; }
        /// <summary>
        /// 是否冻结
        /// </summary>

        public bool IsFreeze { get; set; }
        /// <summary>
        /// 支付渠道
        /// </summary>
        [Required]
        public PayChannels PayChannel { get; set; }
        /// <summary>
        /// 终端
        /// </summary>
        [Required]
        public Terminals Terminal { get; set; }
        /// <summary>
        /// 交易状态
        /// </summary>
        [Required]
        public TransactionStates TransactionState { get; set; }
        /// <summary>
        /// 自定义数据
        /// </summary>
        [MaxLength(500)]
        public string CustomData { get; set; }
        /// <summary>
        /// 交易单号
        /// </summary>
        [MaxLength(50)]
        public string OutTradeNo { get; set; }
        /// <summary>
        /// 支付完成时间
        /// </summary>

        public DateTime? PayTime { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        [MaxLength(2000)]
        public string Exception { get; set; }
    }
}