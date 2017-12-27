using Abp.AutoMapper;
using Magicodes.Admin.Organizations.Dto;

namespace Magicodes.Admin.Models.Users
{
    [AutoMapFrom(typeof(OrganizationUnitDto))]
    public class OrganizationUnitModel : OrganizationUnitDto
    {
        public bool IsAssigned { get; set; }
    }
}