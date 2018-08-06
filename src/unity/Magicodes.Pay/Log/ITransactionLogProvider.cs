using System;
using Magicodes.Admin.Core.Custom.LogInfos;

namespace Magicodes.Pay.Log
{
    /// <summary>
    /// 交易日志提供程序
    /// </summary>
    public interface ITransactionLogProvider
    {
        /// <summary>
        /// 填充交易信息
        /// </summary>
        /// <param name="transactionLog">交易日志</param>
        /// <param name="exception">交易异常</param>
        void Fill(TransactionLog transactionLog, Exception exception = null);
    }
}
