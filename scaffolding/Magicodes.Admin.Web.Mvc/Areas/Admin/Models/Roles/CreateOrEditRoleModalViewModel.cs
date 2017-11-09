using Abp.AutoMapper;
using Magicodes.Admin.Authorization.Roles.Dto;
using Magicodes.Admin.Web.Areas.Admin.Models.Common;

namespace Magicodes.Admin.Web.Areas.Admin.Models.Roles
{
    [AutoMapFrom(typeof(GetRoleForEditOutput))]
    public class CreateOrEditRoleModalViewModel : GetRoleForEditOutput, IPermissionsEditViewModel
    {
        public bool IsEditMode
        {
            get { return Role.Id.HasValue; }
        }

        public CreateOrEditRoleModalViewModel(GetRoleForEditOutput output)
        {
            output.MapTo(this);
        }
    }
}