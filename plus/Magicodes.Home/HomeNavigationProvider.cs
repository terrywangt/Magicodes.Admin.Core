using Abp.Application.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magicodes.Admin.Web.Areas.Admin.Startup;
using Abp.Localization;
using Magicodes.Admin.Web.Startup;

namespace Magicodes.Home
{
    public class HomeNavigationProvider : NavigationProvider
    {

        public override void SetNavigation(INavigationProviderContext context)
        {
            var menu = context.Manager.Menus[FrontEndNavigationProvider.MenuName];
            //清除菜单
            menu.Items.Clear();
            string area = "";
            menu.AddItem(new MenuItemDefinition(
                    FrontEndPageNames.Home,
                    L("Home"),
                    url: ""
                    ));
            //var rootDemoMenu = new MenuItemDefinition("RootDemoMenu",
            //        L("UITheme"),
            //        url: area + "Home",
            //        icon: "icon-screen-desktop");

            
        }

        protected static ILocalizableString L(string name)
        {
            return new LocalizableString(name, HomeConsts.LocalizationSourceName);
        }
    }
}
