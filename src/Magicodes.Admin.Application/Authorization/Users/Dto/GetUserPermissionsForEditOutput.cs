using System.Collections.Generic;
using Magicodes.Admin.Authorization.Permissions.Dto;

namespace Magicodes.Admin.Authorization.Users.Dto
{
    public class GetUserPermissionsForEditOutput
    {
        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}