using Abp.Threading;
using Magicodes.Admin.Core.Custom.LogInfos;

namespace Magicodes.Pay.Log
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static class TransactionLogStoreExtensions
    {
        /// <summary>
        /// 保存交易日志
        /// </summary>
        /// <param name="transactionLogStore"></param>
        /// <param name="transactionLog"></param>
        public static void Save(this ITransactionLogStore transactionLogStore, TransactionLog transactionLog)
        {
            AsyncHelper.RunSync(() => transactionLogStore.SaveAsync(transactionLog));
        }
    }
}
