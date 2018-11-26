using Abp;
using Abp.Dependency;
using Abp.Json;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Pay.Log;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Magicodes.Pay.PaymentCallbacks
{
    /// <summary>
    /// 支付回调管理
    /// </summary>
    public class PaymentCallbackManager : ITransientDependency
    {
        public ILogger Logger { get; set; }

        private readonly TransactionLogHelper _transactionLogHelper;
        public UserManager UserManager { get; set; }
        public IAbpSession AbpSession { get; set; }

        public PaymentCallbackManager(TransactionLogHelper transactionLogHelper)
        {
            _transactionLogHelper = transactionLogHelper;
        }

        /// <summary>
        /// 执行回调逻辑
        /// </summary>
        /// <param name="key">支付业务关键字</param>
        /// <param name="outTradeNo">商户系统的订单号</param>
        /// <param name="totalFee">金额（单位：分）</param>
        /// <param name="transactionId">微信支付订单号</param>
        /// <param name="data">自定义数据</param>
        /// <returns></returns>
        public async Task ExecuteCallback(string key, string outTradeNo, string transactionId, int totalFee, JObject data)
        {
            Logger.Info("正在执行【" + key + "】回调逻辑。data:" + data?.ToJsonString());
            var userIdentifer = UserIdentifier.Parse(data?["uid"]?.ToString());
            using (AbpSession.Use(userIdentifer.TenantId, userIdentifer.UserId))
            {
                //更新交易日志
                await _transactionLogHelper.UpdateAsync(outTradeNo, transactionId, async (unitOfWork, loginfo) =>
                 {
                     //TODO:金额比较
                     //if (loginfo.Currency.CurrencyValue == totalFee)
                     //{
                         
                     //}
                     switch (key)
                     {
                         case "订单支付":
                             {
                                 var orderCode = data?["code"]?.ToString();
                                 //await _orderManager.UpdateOrderPayStatus(orderCode, totalFee);
                                 break;
                             }
                         case "系统充值":
                             {
                                 await UserManager.UpdateRechargeInfo(userIdentifer, totalFee);
                             }
                             break;
                         default:

                             break;
                     }

                 });

            }
        }
    }
}
