using Magicodes.Admin.Editions;
using Magicodes.Admin.Editions.Dto;
using Magicodes.Admin.MultiTenancy.Payments;
using Magicodes.Admin.MultiTenancy.Payments.Dto;

namespace Magicodes.Admin.Web.Models.Payment
{
    public class PaymentViewModel
    {
        public SubscriptionStartType? SubscriptionStartType { get; set; }

        public EditionSelectDto Edition { get; set; }

        public decimal? AdditionalPrice { get; set; }

        public EditionPaymentType EditionPaymentType { get; set; }
        
        public string GetAdditionalData(SubscriptionPaymentGatewayType gateway, string key)
        {
            return Edition.AdditionalData[gateway][key];
        }

        public string GetFormArea()
        {
            if (EditionPaymentType == EditionPaymentType.NewRegistration)
            {
                return "";
            }
                   
            return "Admin";
        }

        public string GetFormPostController()
        {
            if (EditionPaymentType == EditionPaymentType.NewRegistration)
            {
                return "Payment";
            }

            return "SubscriptionManagement";
        }

        public string GetFormAction()
        {
            if (EditionPaymentType == EditionPaymentType.NewRegistration)
            {
                return "ExecutePayment";
            }

            return "PaymentResult";
        }

        public bool IsUpgrading()
        {
            return AdditionalPrice.HasValue && AdditionalPrice.Value > 0;
        }
    }
}
