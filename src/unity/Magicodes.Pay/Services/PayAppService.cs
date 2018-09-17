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

using System;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.UI;
using Magicodes.Alipay;
using Magicodes.Pay.WeChat;
using Magicodes.Pay.Services.Dto;
using Microsoft.AspNetCore.Mvc;
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

        public PayAppService(IClientInfoProvider clientInfoProvider) => _clientInfoProvider = clientInfoProvider;

        public WeChatPayApi WeChatPayApi { get; set; }

        public IAlipayAppService AlipayAppService { get; set; }

        /// <summary>
        ///     支付宝APP支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("AppPay/Alipay")]
        public async Task<string> AliAppPay(AppPayInput input)
        {
            if (AlipayAppService == null)
            {
                throw new UserFriendlyException("支付未开放，请联系管理员！");
            }
            var appPayInput = new Alipay.Dto.AppPayInput
            {
                Body = input.Body,
                Subject = input.Subject,
                //TradeNo = ,
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
        ///     微信APP支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("AppPay/WeChat")]
        public Task<AppPayOutput> WeChatAppPay(AppPayInput input)
        {
            if (WeChatPayApi == null)
            {
                throw new UserFriendlyException("支付未开放，请联系管理员！");
            }
            var appPayInput = new WeChat.Pay.Dto.AppPayInput
            {
                Body = input.Body,
                //OutTradeNo = ,
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
    }
}