using System.Threading.Tasks;
using Abp.Application.Services;
using Magicodes.Admin.MultiTenancy.Dto;
using Magicodes.Admin.MultiTenancy.Payments.Dto;

namespace Magicodes.Admin.MultiTenancy.Payments
{
    public interface IPaymentAppService : IApplicationService
    {
        Task<PaymentInfoDto> GetPaymentInfo(PaymentInfoInput input);

        Task<CreatePaymentResponse> CreatePayment(CreatePaymentDto input);

        Task<ExecutePaymentResponse> ExecutePayment(ExecutePaymentDto input);
    }
}
