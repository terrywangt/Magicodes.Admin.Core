using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Magicodes.Admin.Web.Areas.Admin.Models.Tenants
{
    public class CreateTenantViewModel
    {
        public IReadOnlyList<ComboboxItemDto> EditionItems { get; set; }

        public CreateTenantViewModel(IReadOnlyList<ComboboxItemDto> editionItems)
        {
            EditionItems = editionItems;
        }
    }
}