using System.Collections.Generic;
using Magicodes.Admin.Authorization.Permissions.Dto;

namespace Magicodes.Admin.Web.Areas.Admin.Models.Common
{
    public interface IPermissionsEditViewModel
    {
        List<FlatPermissionDto> Permissions { get; set; }

        List<string> GrantedPermissionNames { get; set; }
    }
}