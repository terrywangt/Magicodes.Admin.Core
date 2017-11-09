using Abp.AutoMapper;
using Magicodes.Admin.Sessions.Dto;

namespace Magicodes.Admin.Web.Views.Shared.Components.TenantChange
{
    [AutoMapFrom(typeof(GetCurrentLoginInformationsOutput))]
    public class TenantChangeViewModel
    {
        public TenantLoginInfoDto Tenant { get; set; }
    }
}