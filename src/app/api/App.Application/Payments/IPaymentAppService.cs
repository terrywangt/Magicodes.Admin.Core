using Abp.Application.Services;
using System.Threading.Tasks;
using Magicodes.App.Application.Payments.Dto;

namespace Magicodes.App.Application.Payments
{
    /// <summary>
    /// 支付
    /// </summary>
    public interface IPaymentAppService : IApplicationService
    {
        /// <summary>
        /// 订单支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<string> CreateOrderPayment(CreateOrderPaymentInput input);

        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<string> Recharge(RechargeInput input);
    }
}