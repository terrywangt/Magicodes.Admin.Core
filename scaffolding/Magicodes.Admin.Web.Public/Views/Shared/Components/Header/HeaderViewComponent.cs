using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Navigation;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Extensions;
using Abp.Localization;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.MultiTenancy;
using Magicodes.Admin.Url;
using Magicodes.Admin.Web.Public.Startup;
using Magicodes.Admin.Web.Session;

namespace Magicodes.Admin.Web.Public.Views.Shared.Components.Header
{
    public class HeaderViewComponent : AdminViewComponent
    {
        private readonly IUserNavigationManager _userNavigationManager;
        private readonly IMultiTenancyConfig _multiTenancyConfig;
        private readonly IAbpSession _abpSession;
        private readonly ILanguageManager _languageManager;
        private readonly ISettingManager _settingManager;
        private readonly IPerRequestSessionCache _sessionCache;
        private readonly IWebUrlService _webUrlService;
        private readonly TenantManager _tenantManager;

        public HeaderViewComponent(
            IUserNavigationManager userNavigationManager, 
            IMultiTenancyConfig multiTenancyConfig,
            IAbpSession abpSession,
            ILanguageManager languageManager, 
            ISettingManager settingManager, 
            IPerRequestSessionCache sessionCache,
            IWebUrlService webUrlService, 
            TenantManager tenantManager)
        {
            _userNavigationManager = userNavigationManager;
            _multiTenancyConfig = multiTenancyConfig;
            _abpSession = abpSession;
            _languageManager = languageManager;
            _settingManager = settingManager;
            _sessionCache = sessionCache;
            _webUrlService = webUrlService;
            _tenantManager = tenantManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string currentPageName = "")
        {
            var tenancyName = "";
            if (_abpSession.TenantId.HasValue)
            {
                var tenant = await _tenantManager.GetByIdAsync(_abpSession.GetTenantId());
                tenancyName = tenant.TenancyName;
            }

            var headerModel = new HeaderViewModel
            {
                LoginInformations = await _sessionCache.GetCurrentLoginInformationsAsync(),
                IsInHostView = !_abpSession.TenantId.HasValue,
                Languages = _languageManager.GetLanguages().Where(l => !l.IsDisabled).ToList(),
                CurrentLanguage = _languageManager.CurrentLanguage,
                Menu = await _userNavigationManager.GetMenuAsync(FrontEndNavigationProvider.MenuName, _abpSession.ToUserIdentifier()),
                CurrentPageName = currentPageName,
                IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled,
                TenantRegistrationEnabled = await _settingManager.GetSettingValueAsync<bool>(AppSettings.TenantManagement.AllowSelfRegistration),
                AdminWebSiteRootAddress = _webUrlService.GetServerRootAddress(tenancyName).EnsureEndsWith('/'),
                WebSiteRootAddress = _webUrlService.GetSiteRootAddress(tenancyName).EnsureEndsWith('/')
            };

            return View(headerModel);
        }
    }
}
