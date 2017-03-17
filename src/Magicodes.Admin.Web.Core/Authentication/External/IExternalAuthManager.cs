using System.Threading.Tasks;
using Abp.Zero.AspNetCore;

namespace Magicodes.Admin.Web.Authentication.External
{
    public interface IExternalAuthManager
    {
        Task<bool> IsValidUser(string provider, string providerKey, string providerAccessCode);

        Task<ExternalLoginUserInfo> GetUserInfo(string provider, string accessCode);
    }
}
