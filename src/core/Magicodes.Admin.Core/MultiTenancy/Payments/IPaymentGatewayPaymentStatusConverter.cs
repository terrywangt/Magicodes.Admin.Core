namespace Magicodes.Admin.MultiTenancy.Payments
{
    public interface IPaymentGatewayPaymentStatusConverter
    {
        SubscriptionPaymentStatus ConvertToSubscriptionPaymentStatus(string externalStatus);
    }
}