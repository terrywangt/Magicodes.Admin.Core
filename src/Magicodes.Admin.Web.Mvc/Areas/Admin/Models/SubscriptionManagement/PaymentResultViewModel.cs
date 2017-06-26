using Abp.AutoMapper;
using Magicodes.Admin.Editions;
using Magicodes.Admin.MultiTenancy.Payments.Dto;

namespace Magicodes.Admin.Web.Areas.Admin.Models.SubscriptionManagement
{
    [AutoMapTo(typeof(ExecutePaymentDto))]
    public class PaymentResultViewModel : SubscriptionPaymentDto
    {
        public EditionPaymentType EditionPaymentType { get; set; }
    }
}