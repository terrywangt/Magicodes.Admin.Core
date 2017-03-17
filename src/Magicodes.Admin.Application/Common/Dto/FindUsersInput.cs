using Magicodes.Admin.Dto;

namespace Magicodes.Admin.Common.Dto
{
    public class FindUsersInput : PagedAndFilteredInputDto
    {
        public int? TenantId { get; set; }
    }
}