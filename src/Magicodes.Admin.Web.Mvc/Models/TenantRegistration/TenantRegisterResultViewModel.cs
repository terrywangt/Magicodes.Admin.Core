using Abp.AutoMapper;
using Magicodes.Admin.MultiTenancy.Dto;

namespace Magicodes.Admin.Web.Models.TenantRegistration
{
    [AutoMapFrom(typeof(RegisterTenantOutput))]
    public class TenantRegisterResultViewModel : RegisterTenantOutput
    {
        public string TenantLoginAddress { get; set; }
    }
}