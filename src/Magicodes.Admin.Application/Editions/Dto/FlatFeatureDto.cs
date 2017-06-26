using Abp.Application.Features;
using Abp.AutoMapper;

namespace Magicodes.Admin.Editions.Dto
{
    [AutoMapFrom(typeof(Feature))]
    public class FlatFeatureDto
    {
        public string ParentName { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string DefaultValue { get; set; }

        public FeatureInputTypeDto InputType { get; set; }
    }
}