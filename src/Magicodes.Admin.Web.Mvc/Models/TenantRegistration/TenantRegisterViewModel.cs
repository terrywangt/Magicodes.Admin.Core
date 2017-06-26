using Magicodes.Admin.Editions;
using Magicodes.Admin.Editions.Dto;
using Magicodes.Admin.Security;
using Magicodes.Admin.MultiTenancy.Payments;
using Magicodes.Admin.MultiTenancy.Payments.Dto;

namespace Magicodes.Admin.Web.Models.TenantRegistration
{
    public class TenantRegisterViewModel
    {
        public PasswordComplexitySetting PasswordComplexitySetting { get; set; }

        public int EditionId { get; set; }

        public string PaymentId { get; set; }

        public SubscriptionPaymentGatewayType? Gateway { get; set; }

        public SubscriptionStartType SubscriptionStartType { get; set; }

        public EditionSelectDto Edition { get; set; }

        public EditionPaymentType EditionPaymentType { get; set; }

        public bool ShowPaymentExpireNotification()
        {
            return !string.IsNullOrEmpty(PaymentId);
        }
    }
}
