using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Authorization.Users.Dto;

namespace Magicodes.Admin.Authorization.Users
{
    public interface IUserLinkAppService : IApplicationService
    {
        Task LinkToUser(LinkToUserInput linkToUserInput);

        Task<PagedResultDto<LinkedUserDto>> GetLinkedUsers(GetLinkedUsersInput input);

        Task<ListResultDto<LinkedUserDto>> GetRecentlyUsedLinkedUsers();

        Task UnlinkUser(UnlinkUserInput input);
    }
}
