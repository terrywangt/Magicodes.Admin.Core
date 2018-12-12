// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : PayAppService.cs
//           description :
//   
//           created by 雪雁 at  2018-08-06 14:40
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using Abp;
using Abp.Auditing;
using Abp.Timing;
using Abp.UI;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Alipay;
using Magicodes.Alipay.Global;
using Magicodes.Pay.Log;
using Magicodes.Pay.PaymentCallbacks;
using Magicodes.Pay.Services.Dto;
using Magicodes.Pay.WeChat;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Abp.Json;
using Castle.Core.Logging;
using Magicodes.Admin.LogInfos;
using AppPayOutput = Magicodes.Pay.WeChat.Pay.Dto.AppPayOutput;

namespace Magicodes.Pay.Services
{
    /// <summary>
    ///     支付服务
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PayAppService : IPayAppService
    {
        private readonly IClientInfoProvider _clientInfoProvider;
        private readonly PaymentCallbackManager _paymentCallbackManager;
        public UserManager UserManager { get; set; }
        public ILogger Logger { get; set; }


        public PayAppService(IClientInfoProvider clientInfoProvider, TransactionLogHelper transactionLogHelper, PaymentCallbackManager paymentCallbackManager)
        {
            _clientInfoProvider = clientInfoProvider;
            _transactionLogHelper = transactionLogHelper;
            _paymentCallbackManager = paymentCallbackManager;
            Logger = NullLogger.Instance;
        }

        public WeChatPayApi WeChatPayApi { get; set; }

        public IAlipayAppService AlipayAppService { get; set; }

        public IGlobalAlipayAppService GlobalAlipayAppService { get; set; }

        private readonly TransactionLogHelper _transactionLogHelper;

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<object> Pay(PayInput input)
        {
            Logger.Debug("准备发起支付：" + input.ToJsonString());
            object output = null;
            Exception exception = null;
            if (input.OutTradeNo == null)
            {
                input.OutTradeNo = GenerateOutTradeNo();
            }

            try
            {
                //TODO:添加客户端请求头判断,支持自动使用PC/H5/APP等支付
                switch (input.PayChannel)
                {
                    case PayChannels.WeChatPay:
                        output = await WeChatAppPay(input);
                        break;
                    case PayChannels.AliPay:
                        output = await AliAppPay(input);
                        break;
                    case PayChannels.GlobalAlipay:
                        output = await GlobalAlipay(input);
                        break;
                    case PayChannels.BalancePay:
                        await BalancePay(input);
                        return null;
                    default:
                        throw new UserFriendlyException("当前不支持此种类型的支付！");
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (input.PayChannel != PayChannels.BalancePay)
            {
                //创建交易日志
                await CreateToPayTransactionInfo(input, exception);
                if (exception != null)
                {
                    Logger.Error("支付失败！", exception);
                    throw new UserFriendlyException("支付异常，请联系客服人员或稍后再试！");
                }
            }

            return output;
        }

        /// <summary>
        ///     支付宝APP支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[HttpPost("AppPay/Alipay")]
        protected async Task<string> AliAppPay(AppPayInput input)
        {
            if (AlipayAppService == null)
            {
                throw new UserFriendlyException("支付未开放，请联系管理员！");
            }
            var appPayInput = new Alipay.Dto.AppPayInput
            {
                Body = input.Body,
                Subject = input.Subject,
                TradeNo = input.OutTradeNo,
                PassbackParams = input.CustomData,
                TotalAmount = input.TotalAmount
            };
            try
            {
                var appPayOutput = await AlipayAppService.AppPay(appPayInput);
                return appPayOutput.Response.Body;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        /// <summary>
        ///     支付宝APP支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[HttpPost("AppPay/GlobalAlipay")]
        protected async Task<PayOutput> GlobalAlipay(AppPayInput input)
        {
            if (GlobalAlipayAppService == null)
            {
                throw new UserFriendlyException("支付未开放，请联系管理员！");
            }
            var payInput = new Alipay.Global.Dto.PayInput
            {
                Body = input.Body,
                Subject = input.Subject,
                TradeNo = input.OutTradeNo,
                //PassbackParams = input.CustomData,
                TotalFee = input.TotalAmount,
            };
            try
            {
                return await GlobalAlipayAppService.Pay(payInput);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        /// <summary>
        ///     微信APP支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[HttpPost("AppPay/WeChat")]
        protected Task<AppPayOutput> WeChatAppPay(AppPayInput input)
        {
            if (WeChatPayApi == null)
            {
                throw new UserFriendlyException("支付未开放，请联系管理员！");
            }
            var appPayInput = new WeChat.Pay.Dto.AppPayInput
            {
                Body = input.Body,
                OutTradeNo = input.OutTradeNo,
                Attach = input.CustomData,
                TotalFee = input.TotalAmount,
                SpbillCreateIp = _clientInfoProvider?.ClientIpAddress
            };
            try
            {
                var appPayOutput = WeChatPayApi.AppPay(appPayInput);
                return Task.FromResult(appPayOutput);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        /// <summary>
        /// 余额支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected async Task BalancePay(PayInput input)
        {
            var data = JsonConvert.DeserializeObject<JObject>(input.CustomData);
            var uid = data["uid"]?.ToString();
            var log = await CreateToPayTransactionInfo(input);

            if (data["key"]?.ToString() == "系统充值")
            {
                throw new UserFriendlyException("余额支付不支持此业务！");
            }

            var userIdentifer = UserIdentifier.Parse(uid);
            await UserManager.UpdateRechargeInfo(userIdentifer, (int)(-input.TotalAmount * 100));
            await _paymentCallbackManager.ExecuteCallback(data["key"]?.ToString(), log.OutTradeNo, log.TransactionId, (int)(input.TotalAmount * 100), data);
        }

        /// <summary>
        /// 创建交易日志
        /// </summary>
        /// <returns></returns>
        private async Task<TransactionLog> CreateToPayTransactionInfo(PayInput input, Exception exception = null)
        {
            var transactionInfo = new TransactionInfo()
            {
                Amount = input.TotalAmount,
                CustomData = input.CustomData,
                OutTradeNo = input.OutTradeNo ?? GenerateOutTradeNo(),
                PayChannel = input.PayChannel,
                Subject = input.Subject,
                TransactionState = TransactionStates.NotPay,
                //TransactionId = "",
                Exception = exception
            };
            TransactionLog transactionLog = null;
            if (input.PayChannel == PayChannels.GlobalAlipay)
            {
                //添加货币符号，以支持国际支付
                var config = Magicodes.Alipay.Global.GlobalAlipayAppService.GetPayConfigFunc();
                transactionLog = _transactionLogHelper.CreateTransactionLog(transactionInfo, config.Currency);
            }
            else
            {
                transactionLog = _transactionLogHelper.CreateTransactionLog(transactionInfo);
            }
            await _transactionLogHelper.SaveAsync(transactionLog);
            return transactionLog;
        }

        /// <summary>
        /// 生成交易单号
        /// </summary>
        /// <returns></returns>
        private string GenerateOutTradeNo()
        {
            var code = RandomHelper.GetRandom(100, 999);
            return $"M{Clock.Now:yyyyMMddHHmmss}{code}";
        }
    }
}