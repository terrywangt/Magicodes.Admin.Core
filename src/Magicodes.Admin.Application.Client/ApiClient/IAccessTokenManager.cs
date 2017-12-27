using System.Threading.Tasks;
using Magicodes.Admin.ApiClient.Models;

namespace Magicodes.Admin.ApiClient
{
    public interface IAccessTokenManager
    {
        Task<string> GetAccessTokenAsync();
         
        Task<AbpAuthenticateResultModel> LoginAsync();

        void Logout();

        bool IsUserLoggedIn { get; }
    }
}