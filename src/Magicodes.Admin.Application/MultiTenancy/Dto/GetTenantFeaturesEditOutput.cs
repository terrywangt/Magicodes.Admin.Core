using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Editions.Dto;

namespace Magicodes.Admin.MultiTenancy.Dto
{
    public class GetTenantFeaturesEditOutput
    {
        public List<NameValueDto> FeatureValues { get; set; }

        public List<FlatFeatureDto> Features { get; set; }
    }
}