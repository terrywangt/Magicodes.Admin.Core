using System.Threading.Tasks;
using Magicodes.Admin.Authorization.Accounts.Dto;

namespace Magicodes.Admin.Authorization.Accounts
{
    public class ProxyAccountAppService : ProxyAppServiceBase, IAccountAppService
    {
        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            return await ApiClient.PostAnonymousAync<IsTenantAvailableOutput>(GetEndpoint(nameof(IsTenantAvailable)), input);
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            return await ApiClient.PostAnonymousAync<RegisterOutput>(GetEndpoint(nameof(Register)), input);
        }

        public async Task SendPasswordResetCode(SendPasswordResetCodeInput input)
        {
            await ApiClient.PostAnonymousAync(GetEndpoint(nameof(SendPasswordResetCode)), input);
        }

        public async Task<ResetPasswordOutput> ResetPassword(ResetPasswordInput input)
        {
            return await ApiClient.PostAnonymousAync<ResetPasswordOutput>(GetEndpoint(nameof(ResetPassword)), input);
        }

        public async Task SendEmailActivationLink(SendEmailActivationLinkInput input)
        {
            await ApiClient.PostAnonymousAync(GetEndpoint(nameof(SendEmailActivationLink)), input);
        }

        public async Task ActivateEmail(ActivateEmailInput input)
        {
            await ApiClient.PostAnonymousAync(GetEndpoint(nameof(ActivateEmail)), input);
        }

        public async Task<ImpersonateOutput> Impersonate(ImpersonateInput input)
        {
            return await ApiClient.PostAync<ImpersonateOutput>(GetEndpoint(nameof(Impersonate)), input);
        }

        public async Task<ImpersonateOutput> BackToImpersonator()
        {
            return await ApiClient.PostAnonymousAync<ImpersonateOutput>(GetEndpoint(nameof(BackToImpersonator)));
        }

        public async Task<SwitchToLinkedAccountOutput> SwitchToLinkedAccount(SwitchToLinkedAccountInput input)
        {
            return await ApiClient.PostAnonymousAync<SwitchToLinkedAccountOutput>(GetEndpoint(nameof(SwitchToLinkedAccount)));
        }
    }
}