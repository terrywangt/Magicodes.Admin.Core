using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Threading;
using Magicodes.Admin.Common.Dto;
using Magicodes.Admin.Editions.Dto;

namespace Magicodes.Admin.Common
{
    public class ProxyCommonLookupAppService : ProxyAppServiceBase, ICommonLookupAppService
    {
        public async Task<ListResultDto<SubscribableEditionComboboxItemDto>> GetEditionsForCombobox(bool onlyFreeItems = false)
        {
            return await ApiClient.GetAsync<ListResultDto<SubscribableEditionComboboxItemDto>>(GetEndpoint(nameof(GetEditionsForCombobox)));
        }

        public async Task<PagedResultDto<NameValueDto>> FindUsers(FindUsersInput input)
        {
            return await ApiClient.PostAync<PagedResultDto<NameValueDto>>(GetEndpoint(nameof(FindUsers)), input);
        }

        public GetDefaultEditionNameOutput GetDefaultEditionName()
        {
            return AsyncHelper.RunSync(() =>
                ApiClient.GetAsync<GetDefaultEditionNameOutput>(GetEndpoint(nameof(GetDefaultEditionName))));
        }
    }
}
