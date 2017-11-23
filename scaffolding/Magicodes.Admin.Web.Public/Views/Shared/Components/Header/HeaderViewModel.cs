using System.Collections.Generic;
using Abp.Application.Navigation;
using Abp.Extensions;
using Abp.Localization;
using Magicodes.Admin.Sessions.Dto;

namespace Magicodes.Admin.Web.Public.Views.Shared.Components.Header
{
    public class HeaderViewModel
    {
        public GetCurrentLoginInformationsOutput LoginInformations { get; set; }

        public IReadOnlyList<LanguageInfo> Languages { get; set; }

        public LanguageInfo CurrentLanguage { get; set; }

        public UserMenu Menu { get; set; }

        public string CurrentPageName { get; set; }

        public bool IsMultiTenancyEnabled { get; set; }

        public bool TenantRegistrationEnabled { get; set; }

        public bool IsInHostView { get; set; }

        public string AdminWebSiteRootAddress { get; set; }

        public string WebSiteRootAddress { get; set; }

        public string GetShownLoginName()
        {
            if (!IsMultiTenancyEnabled)
            {
                return LoginInformations.User.UserName;
            }

            return LoginInformations.Tenant == null
                ? ".\\" + LoginInformations.User.UserName
                : LoginInformations.Tenant.TenancyName + "\\" + LoginInformations.User.UserName;
        }

        public string GetLogoUrl(string appPath)
        {
            if (!IsMultiTenancyEnabled || LoginInformations?.Tenant?.LogoId == null)
            {
                return appPath + "Common/Images/app-logo-on-light.png";
            }

            return AdminWebSiteRootAddress.EnsureEndsWith('/') + "TenantCustomization/GetLogo?id=" + LoginInformations.Tenant.LogoId;
        }
    }
}