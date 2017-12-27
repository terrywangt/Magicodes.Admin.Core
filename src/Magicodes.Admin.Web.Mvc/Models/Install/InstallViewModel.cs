using System.Collections.Generic;
using Abp.Localization;
using Magicodes.Admin.Install.Dto;

namespace Magicodes.Admin.Web.Models.Install
{
    public class InstallViewModel
    {
        public List<ApplicationLanguage> Languages { get; set; }

        public AppSettingsJsonDto AppSettingsJson { get; set; }
    }
}
