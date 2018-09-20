using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Magicodes.Admin.Core.Custom.LogInfos;
using System;

namespace Admin.Application.Custom.LogInfos.Dto
{
    /// <summary>
    ///  交易日志列表Dto
    /// </summary>
    [AutoMapFrom(typeof(TransactionLog))]
    public partial class TransactionLogListDto : EntityDto<long>
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        public string CultureValue { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// 客户端Ip
        /// </summary>
        public string ClientIpAddress { get; set; }
        /// <summary>
        /// 客户端名称
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// 是否冻结
        /// </summary>
        public bool IsFreeze { get; set; }
        /// <summary>
        /// 支付渠道
        /// </summary>
        public PayChannels PayChannel { get; set; }
        /// <summary>
        /// 终端
        /// </summary>
        public Terminals Terminal { get; set; }
        /// <summary>
        /// 交易状态
        /// </summary>
        public TransactionStates TransactionState { get; set; }
        /// <summary>
        /// 自定义数据
        /// </summary>
        public string CustomData { get; set; }
        /// <summary>
        /// 交易单号
        /// </summary>
        public string OutTradeNo { get; set; }
        /// <summary>
        /// 支付完成时间
        /// </summary>
        public DateTime? PayTime { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string Exception { get; set; }


        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}