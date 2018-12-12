using System;
using Abp.AutoMapper;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Excel;

namespace Magicodes.Admin.LogInfos.Dto
{
    /// <summary>
    ///  交易日志导出Dto
    /// </summary>
    [ExcelExporter(Name = "交易日志", TableStyle = "Light10")]
    [AutoMapFrom(typeof(TransactionLog))]
    public class TransactionLogExportDto
    {
        /// <summary>
        /// 金额
        /// </summary>
        [ExporterHeader(DisplayName = "金额", IsAutoFit = true)]
        public decimal Amount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ExporterHeader(Format = "yyyy-MM-dd HH:mm:ss")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 客户端Ip
        /// </summary>
        [ExporterHeader(DisplayName = "客户端Ip", IsAutoFit = true)]
        public string ClientIpAddress { get; set; }

        /// <summary>
        /// 客户端名称
        /// </summary>
        [ExporterHeader(DisplayName = "客户端名称", IsAutoFit = true)]
        public string ClientName { get; set; }

        /// <summary>
        /// 支付渠道
        /// </summary>
        [ExporterHeader(DisplayName = "支付渠道", IsAutoFit = true)]
        public PayChannels PayChannel { get; set; }

        /// <summary>
        /// 终端
        /// </summary>
        [ExporterHeader(DisplayName = "终端", IsAutoFit = true)]
        public Terminals Terminal { get; set; }

        /// <summary>
        /// 交易状态
        /// </summary>
        [ExporterHeader(DisplayName = "交易状态", IsAutoFit = true)]
        public TransactionStates TransactionState { get; set; }

        /// <summary>
        /// 自定义数据
        /// </summary>
        [ExporterHeader(DisplayName = "自定义数据", IsAutoFit = true)]
        public string CustomData { get; set; }

        /// <summary>
        /// 交易单号
        /// </summary>
        [ExporterHeader(DisplayName = "交易单号", IsAutoFit = true)]
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 支付完成时间
        /// </summary>
        [ExporterHeader(Format = "yyyy-MM-dd HH:mm:ss")]
        public DateTime? PayTime { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        [ExporterHeader(DisplayName = "异常信息", IsAutoFit = true)]
        public string Exception { get; set; }

    }
}