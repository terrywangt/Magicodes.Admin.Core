using Abp.Application.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magicodes.Admin.Web.Areas.Admin.Startup;
using Abp.Localization;
using Magicodes.WeChat.Core;
using Magicodes.WeChat.Core.Authorization;

namespace Magicodes.WeChat.Web.Mvc
{
    public class WeChatNavigationProvider : NavigationProvider
    {

        public override void SetNavigation(INavigationProviderContext context)
        {
            var menu = context.Manager.Menus[AdminNavigationProvider.MenuName];
            string area = WeChatConsts.AdminAreaName + "/";
            var rootDemoMenu = new MenuItemDefinition("WeChatRootMenu",
                    L("Pages"),
                    url: area + "Home",
                    icon: "icon-screen-desktop",
                    requiredPermissionName: WeChatPermissions.WeChatPermissions_Pages,
                    requiresAuthentication: true);

            rootDemoMenu.AddItem(
                new MenuItemDefinition("WeChatUsersMenu",
                    L("WeChatPermissions_Pages_Tenants_WeChatUsers"),
                    url: area + "WeChatUsers",
                    icon: "icon-grid")
              );

            menu.AddItem(rootDemoMenu);
        }

        protected static ILocalizableString L(string name)
        {
            return new LocalizableString(name, WeChatConsts.LocalizationSourceName);
        }
    }
}
