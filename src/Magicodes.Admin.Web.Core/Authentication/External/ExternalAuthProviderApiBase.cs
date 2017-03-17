using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Zero.AspNetCore;

namespace Magicodes.Admin.Web.Authentication.External
{
    public abstract class ExternalAuthProviderApiBase : IExternalAuthProviderApi, ITransientDependency
    {
        public ExternalLoginProviderInfo ProviderInfo { get; set; }

        public void Initialize(ExternalLoginProviderInfo providerInfo)
        {
            ProviderInfo = providerInfo;
        }

        public async Task<bool> IsValidUser(string userId, string accessCode)
        {
            var userInfo = await GetUserInfo(accessCode);
            return userInfo.LoginInfo.ProviderKey == userId;
        }

        public abstract Task<ExternalLoginUserInfo> GetUserInfo(string accessCode);
    }
}