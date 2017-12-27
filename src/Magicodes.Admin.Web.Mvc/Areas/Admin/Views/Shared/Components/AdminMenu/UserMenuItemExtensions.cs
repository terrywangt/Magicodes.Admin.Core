using Abp.Application.Navigation;
using Abp.AspNetZeroCore.Web.Url;

namespace Magicodes.Admin.Web.Areas.Admin.Views.Shared.Components.AdminSideBar
{
    public static class UserMenuItemExtensions
    {
        public static bool IsMenuActive(this UserMenuItem menuItem, string currentPageName)
        {
            if (menuItem.Name == currentPageName)
            {
                return true;
            }

            if (menuItem.Items != null)
            {
                foreach (var subMenuItem in menuItem.Items)
                {
                    if (subMenuItem.IsMenuActive(currentPageName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static string CalculateUrl(this UserMenuItem menuItem, string applicationPath)
        {
            if (string.IsNullOrEmpty(menuItem.Url))
            {
                return applicationPath;
            }

            if (UrlChecker.IsRooted(menuItem.Url))
            {
                return menuItem.Url;
            }

            return applicationPath + menuItem.Url;
        }
    }
}
