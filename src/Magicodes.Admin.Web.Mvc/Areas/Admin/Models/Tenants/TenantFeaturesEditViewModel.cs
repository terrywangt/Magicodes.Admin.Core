using Abp.AutoMapper;
using Magicodes.Admin.MultiTenancy;
using Magicodes.Admin.MultiTenancy.Dto;
using Magicodes.Admin.Web.Areas.Admin.Models.Common;

namespace Magicodes.Admin.Web.Areas.Admin.Models.Tenants
{
    [AutoMapFrom(typeof (GetTenantFeaturesEditOutput))]
    public class TenantFeaturesEditViewModel : GetTenantFeaturesEditOutput, IFeatureEditViewModel
    {
        public Tenant Tenant { get; set; }

        public TenantFeaturesEditViewModel(Tenant tenant, GetTenantFeaturesEditOutput output)
        {
            Tenant = tenant;
            output.MapTo(this);
        }
    }
}