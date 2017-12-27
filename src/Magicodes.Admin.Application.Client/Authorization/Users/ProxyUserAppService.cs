using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Authorization.Users.Dto;
using Magicodes.Admin.Dto;

namespace Magicodes.Admin.Authorization.Users
{
    public class ProxyUserAppService : ProxyAppServiceBase, IUserAppService
    {
        public async Task<PagedResultDto<UserListDto>> GetUsers(GetUsersInput input)
        {
            return await ApiClient.GetAsync<PagedResultDto<UserListDto>>(GetEndpoint(nameof(GetUsers)), input);
        }

        public async Task<FileDto> GetUsersToExcel()
        {
            return await ApiClient.GetAsync<FileDto>(GetEndpoint(nameof(GetUsersToExcel)));
        }

        public async Task<GetUserForEditOutput> GetUserForEdit(NullableIdDto<long> input)
        {
            return await ApiClient.GetAsync<GetUserForEditOutput>(GetEndpoint(nameof(GetUserForEdit)), input);
        }

        public async Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEdit(EntityDto<long> input)
        {
            return await ApiClient.GetAsync<GetUserPermissionsForEditOutput>(GetEndpoint(nameof(GetUserPermissionsForEdit)), input);
        }

        public async Task ResetUserSpecificPermissions(EntityDto<long> input)
        {
            await ApiClient.PostAync(GetEndpoint(nameof(ResetUserSpecificPermissions)), input);
        }

        public async Task UpdateUserPermissions(UpdateUserPermissionsInput input)
        {
            await ApiClient.PutAsync(GetEndpoint(nameof(UpdateUserPermissions)), input);
        }

        public async Task CreateOrUpdateUser(CreateOrUpdateUserInput input)
        {
            await ApiClient.PostAync(GetEndpoint(nameof(CreateOrUpdateUser)), input);
        }

        public async Task DeleteUser(EntityDto<long> input)
        {
            await ApiClient.DeleteAsync(GetEndpoint(nameof(DeleteUser)), input);
        }

        public async Task UnlockUser(EntityDto<long> input)
        {
            await ApiClient.PostAync(GetEndpoint(nameof(UnlockUser)), input);
        }
    }
}