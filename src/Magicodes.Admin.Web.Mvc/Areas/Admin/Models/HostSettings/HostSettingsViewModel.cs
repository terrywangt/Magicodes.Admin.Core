using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Configuration.Host.Dto;

namespace Magicodes.Admin.Web.Areas.Admin.Models.HostSettings
{
    public class HostSettingsViewModel
    {
        public HostSettingsEditDto Settings { get; set; }

        public List<ComboboxItemDto> EditionItems { get; set; }

        public List<ComboboxItemDto> TimezoneItems { get; set; }
    }
}