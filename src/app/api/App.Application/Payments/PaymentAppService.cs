using Abp.Domain.Uow;
using Abp.Json;
using Abp.Runtime.Session;
using Abp.UI;
using Magicodes.App.Application.Payments.Dto;
using Magicodes.Pay.Services;
using Magicodes.Pay.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Magicodes.App.Application.Payments
{
    /// <summary>
    ///     统一支付
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PaymentAppService : AppServiceBase, IPaymentAppService
    {
        private readonly IPayAppService _payAppService;

        public PaymentAppService(IPayAppService payAppService)
        {
            _payAppService = payAppService;
        }

        /// <summary>
        ///     订单支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Order")]
        [UnitOfWork(IsDisabled = true)]
        public async Task<string> CreateOrderPayment(CreateOrderPaymentInput input)
        {
            //var orderInfo = await CheckOrder(input.OrderCode);
            //if (!_orderManager.IsAvailableForPay(orderInfo))
            //{
            //    throw new UserFriendlyException("订单异常，请联系客服人员！");
            //}
            //var payInput = new PayInput
            //{
            //    Body = "心莱科技订阅服务",
            //    TotalAmount = orderInfo.PayableAmount,
            //    PayChannel = input.PayChannel,
            //    Subject = "订单支付",
            //    //OutTradeNo = orderInfo.OrderCode,
            //    CustomData = new
            //    {
            //        key = "订单支付",
            //        code = orderInfo.OrderCode,
            //        uid = AbpSession.ToUserIdentifier().ToUserIdentifierString()
            //    }.ToJsonString()
            //};

            //var result = await _payAppService.Pay(payInput);
            //return (string)(result is string ? result : result.ToJsonString());
            throw new UserFriendlyException("暂未实现！");
        }

        /// <summary>
        ///     充值
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Recharge")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<string> Recharge(RechargeInput input)
        {
            var payInput = new PayInput
            {
                Body = "系统充值",
                TotalAmount = input.TotalAmount,
                Subject = "系统充值",
                CustomData = new
                {
                    key = "系统充值",
                    uid = AbpSession.ToUserIdentifier().ToUserIdentifierString()
                }.ToJsonString()
            };
            //使用统一支付接口
            var result = await _payAppService.Pay(payInput);
            return (string) (result is string ? result : result.ToJsonString());
        }
    }
}