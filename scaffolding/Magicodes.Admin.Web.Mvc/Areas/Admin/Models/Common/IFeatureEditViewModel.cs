using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Editions.Dto;

namespace Magicodes.Admin.Web.Areas.Admin.Models.Common
{
    public interface IFeatureEditViewModel
    {
        List<NameValueDto> FeatureValues { get; set; }

        List<FlatFeatureDto> Features { get; set; }
    }
}