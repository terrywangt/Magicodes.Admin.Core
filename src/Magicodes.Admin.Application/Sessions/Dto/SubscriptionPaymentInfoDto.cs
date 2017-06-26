using Abp.AutoMapper;
using Magicodes.Admin.MultiTenancy.Payments;

namespace Magicodes.Admin.Sessions.Dto
{
    [AutoMapFrom(typeof(SubscriptionPayment))]
    public class SubscriptionPaymentInfoDto
    {
        public decimal Amount { get; set; }
    }
}