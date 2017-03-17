using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Authorization.Permissions.Dto;

namespace Magicodes.Admin.Authorization.Permissions
{
    public interface IPermissionAppService : IApplicationService
    {
        ListResultDto<FlatPermissionWithLevelDto> GetAllPermissions();
    }
}
