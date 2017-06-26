using System.Collections.Generic;
using Magicodes.Admin.Editions.Dto;

namespace Magicodes.Admin.Web.Areas.Admin.Models.Tenants
{
    public class TenantIndexViewModel
    {
        public List<SubscribableEditionComboboxItemDto> EditionItems { get; set; }
    }
}