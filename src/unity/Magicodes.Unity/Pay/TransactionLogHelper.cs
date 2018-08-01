using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Auditing;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using Magicodes.Admin.Core.Custom.LogInfos;

namespace Magicodes.Unity.Pay
{
    public class TransactionLogHelper : ITransientDependency
    {
        public ILogger Logger { get; set; }
        public IAbpSession AbpSession { get; set; }

        private readonly ITransactionLogStore _transactionLogStore;

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        private readonly ITransactionLogProvider _transactionLogProvider;

        public TransactionLogHelper(ITransactionLogProvider transactionLogProvider, IUnitOfWorkManager unitOfWorkManager, ITransactionLogStore transactionLogStore)
        {
            _transactionLogProvider = transactionLogProvider;
            _unitOfWorkManager = unitOfWorkManager;
            _transactionLogStore = transactionLogStore;

            AbpSession = NullAbpSession.Instance;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// 创建交易日志
        /// </summary>
        /// <param name="transactionInfo"></param>
        /// <returns></returns>
        public TransactionLog CreaTransactionLog(TransactionInfo transactionInfo)
        {
            var log = new TransactionLog()
            {
                TenantId = AbpSession.TenantId,
                CreatorUserId = AbpSession.UserId,
                Amount = transactionInfo.Amount,
                CustomData = transactionInfo.CustomData,
                OutTradeNo = transactionInfo.OutTradeNo,
                PayChannel = transactionInfo.PayChannel,
                TransactionState = transactionInfo.TransactionState,
                TransactionId = transactionInfo.TransactionId,
                PayTime = transactionInfo.PayTime
            };
            try
            {
                _transactionLogProvider.Fill(log, transactionInfo.Exception);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString(), ex);
            }
            return log;
        }

        /// <summary>
        /// 保存交易日志
        /// </summary>
        /// <param name="transactionLog"></param>
        public void Save(TransactionLog transactionLog)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                _transactionLogStore.Save(transactionLog);
                uow.Complete();
            }
        }

        /// <summary>
        /// 提交交易日志
        /// </summary>
        /// <param name="transactionLog"></param>
        /// <returns></returns>
        public async Task SaveAsync(TransactionLog transactionLog)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                await _transactionLogStore.SaveAsync(transactionLog);
                await uow.CompleteAsync();
            }
        }


    }
}
