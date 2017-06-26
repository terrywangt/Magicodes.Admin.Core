using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Magicodes.Admin.Editions.Dto;
using Magicodes.Admin.Web.Areas.Admin.Models.Common;

namespace Magicodes.Admin.Web.Areas.Admin.Models.Editions
{
    [AutoMapFrom(typeof(GetEditionEditOutput))]
    public class CreateOrEditEditionModalViewModel : GetEditionEditOutput, IFeatureEditViewModel
    {
        public bool IsEditMode => Edition.Id.HasValue;

        public IReadOnlyList<ComboboxItemDto> EditionItems { get; set; }

        public IReadOnlyList<ComboboxItemDto> FreeEditionItems { get; set; }

        public CreateOrEditEditionModalViewModel(GetEditionEditOutput output, IReadOnlyList<ComboboxItemDto> editionItems, IReadOnlyList<ComboboxItemDto> freeEditionItems)
        {
            EditionItems = editionItems;
            FreeEditionItems = freeEditionItems;
            output.MapTo(this);
        }
    }
}