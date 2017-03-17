using System.Threading.Tasks;
using Abp.Application.Navigation;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.Web.Session;
using Magicodes.Admin.Web.Startup;

namespace Magicodes.Admin.Web.Views.Shared.Components.Header
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IUserNavigationManager _userNavigationManager;
        private readonly IMultiTenancyConfig _multiTenancyConfig;
        private readonly IAbpSession _abpSession;
        private readonly ILanguageManager _languageManager;
        private readonly ISettingManager _settingManager;
        private readonly IPerRequestSessionCache _sessionCache;

        public HeaderViewComponent(
            IUserNavigationManager userNavigationManager, 
            IMultiTenancyConfig multiTenancyConfig,
            IAbpSession abpSession,
            ILanguageManager languageManager, 
            ISettingManager settingManager, 
            IPerRequestSessionCache sessionCache)
        {
            _userNavigationManager = userNavigationManager;
            _multiTenancyConfig = multiTenancyConfig;
            _abpSession = abpSession;
            _languageManager = languageManager;
            _settingManager = settingManager;
            _sessionCache = sessionCache;
        }

        public async Task<IViewComponentResult> InvokeAsync(string currentPageName = "")
        {
            var headerModel = new HeaderViewModel
            {
                LoginInformations = await _sessionCache.GetCurrentLoginInformationsAsync(),
                IsInHostView = !_abpSession.TenantId.HasValue,
                Languages = _languageManager.GetLanguages(),
                CurrentLanguage = _languageManager.CurrentLanguage,
                Menu = await _userNavigationManager.GetMenuAsync(FrontEndNavigationProvider.MenuName, _abpSession.ToUserIdentifier()),
                CurrentPageName = currentPageName,
                IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled,
                TenantRegistrationEnabled = await _settingManager.GetSettingValueAsync<bool>(AppSettings.TenantManagement.AllowSelfRegistration)
            };

            return View(headerModel);
        }
    }
}
