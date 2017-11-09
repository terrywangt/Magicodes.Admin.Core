using System.Collections.Generic;
using Magicodes.Admin.Organizations.Dto;

namespace Magicodes.Admin.Web.Areas.Admin.Models.Common
{
    public interface IOrganizationUnitsEditViewModel
    {
        List<OrganizationUnitDto> AllOrganizationUnits { get; set; }

        List<string> MemberedOrganizationUnits { get; set; }
    }
}