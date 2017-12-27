using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Navigation;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Magicodes.Admin.MultiTenancy;
using Magicodes.Admin.Web.Areas.Admin.Models.Layout;
using Magicodes.Admin.Web.Areas.Admin.Startup;
using Magicodes.Admin.Web.Theme;
using Magicodes.Admin.Web.Views;

namespace Magicodes.Admin.Web.Areas.Admin.Views.Shared.Components.AdminMenu
{
    public class AdminMenuViewComponent : AdminViewComponent
    {
        private readonly IUserNavigationManager _userNavigationManager;
        private readonly IAbpSession _abpSession;
        private readonly TenantManager _tenantManager;
        private readonly IUiThemeCustomizer _uiThemeCustomizer;

        public AdminMenuViewComponent(
            IUserNavigationManager userNavigationManager,
            IAbpSession abpSession,
            TenantManager tenantManager, 
            IUiThemeCustomizer uiThemeCustomizer)
        {
            _userNavigationManager = userNavigationManager;
            _abpSession = abpSession;
            _tenantManager = tenantManager;
            _uiThemeCustomizer = uiThemeCustomizer;
        }

        public async Task<IViewComponentResult> InvokeAsync(string currentPageName = null)
        {
            var model = new MenuViewModel
            {
                Menu = await _userNavigationManager.GetMenuAsync(AdminNavigationProvider.MenuName, _abpSession.ToUserIdentifier()),
                CurrentPageName = currentPageName
            };

            if (AbpSession.TenantId == null)
            {
                return GetView(model);
            }

            var tenant = await _tenantManager.GetByIdAsync(AbpSession.TenantId.Value);
            if (tenant.EditionId.HasValue)
            {
                return GetView(model);
            }

            var subscriptionManagement = FindMenuItemOrNull(model.Menu.Items, AdminPageNames.Tenant.SubscriptionManagement);
            if (subscriptionManagement != null)
            {
                subscriptionManagement.IsVisible = false;
            }

            return GetView(model);
        }

        public UserMenuItem FindMenuItemOrNull(IList<UserMenuItem> userMenuItems, string name)
        {
            if (userMenuItems == null)
            {
                return null;
            }

            foreach (var menuItem in userMenuItems)
            {
                if (menuItem.Name == name)
                {
                    return menuItem;
                }

                var found = FindMenuItemOrNull(menuItem.Items, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private IViewComponentResult GetView(MenuViewModel model)
        {
            if (_uiThemeCustomizer.IsLeftMenuUsed)
            {
                return View("Default", model);
            }

            return View("Top", model);
        }
    }
}
