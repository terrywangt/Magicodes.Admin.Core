using Abp.AutoMapper;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Authorization.Users.Dto;
using Magicodes.Admin.Web.Areas.Admin.Models.Common;

namespace Magicodes.Admin.Web.Areas.Admin.Models.Users
{
    [AutoMapFrom(typeof(GetUserPermissionsForEditOutput))]
    public class UserPermissionsEditViewModel : GetUserPermissionsForEditOutput, IPermissionsEditViewModel
    {
        public User User { get; private set; }

        public UserPermissionsEditViewModel(GetUserPermissionsForEditOutput output, User user)
        {
            User = user;
            output.MapTo(this);
        }
    }
}