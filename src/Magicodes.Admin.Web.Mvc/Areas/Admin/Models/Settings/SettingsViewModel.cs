using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Configuration.Tenants.Dto;

namespace Magicodes.Admin.Web.Areas.Admin.Models.Settings
{
    public class SettingsViewModel
    {
        public TenantSettingsEditDto Settings { get; set; }
        
        public List<ComboboxItemDto> TimezoneItems { get; set; }
    }
}