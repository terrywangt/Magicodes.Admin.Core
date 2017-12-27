using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Web.Models.AbpUserConfiguration;
using Magicodes.Admin.ApiClient;

namespace Magicodes.Admin.Configuration
{
    public class UserConfigurationService : ITransientDependency
    {
        private readonly AbpApiClient _apiClient;

        public UserConfigurationService(AbpApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<AbpUserConfigurationDto> GetAsync(bool isUserLoggedIn)
        {
            const string endpoint = "AbpUserConfiguration/GetAll";

            if (isUserLoggedIn)
            {
                return await _apiClient.GetAsync<AbpUserConfigurationDto>(endpoint);
            }

            return await _apiClient.GetAnonymousAsync<AbpUserConfigurationDto>(endpoint);
        }
    }
}