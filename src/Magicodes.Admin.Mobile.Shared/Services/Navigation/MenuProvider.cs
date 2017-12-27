using System.Collections.Generic;
using Abp.Dependency;
using MvvmHelpers;
using Magicodes.Admin.Localization;
using Magicodes.Admin.Models.NavigationMenu;
using Magicodes.Admin.Services.Permission;
using Magicodes.Admin.Views;

namespace Magicodes.Admin.Services.Navigation
{
    public class MenuProvider : ISingletonDependency, IMenuProvider
    {
        /* For more icons:
            https://material.io/icons/
        */
        private readonly ObservableRangeCollection<NavigationMenuItem> _menuItems = new ObservableRangeCollection<NavigationMenuItem>
        {
            new NavigationMenuItem
            {
                Title = L.Localize("Tenants"),
                Icon = "Tenants.png",
                ViewType = typeof(TenantsView),
                RequiredPermissionName = PermissionKey.Tenants,
            },
            new NavigationMenuItem
            {
                Title = L.Localize("Users"),
                Icon = "UserList.png",
                ViewType = typeof(UsersView),
                RequiredPermissionName = PermissionKey.Users,
            }
            
            /*This is a sample menu item to guide how to add a new item.
                ,new NavigationMenuItem
                {
                    Title = "Sample View",
                    Icon = "MyIcon.png",
                    TargetType = typeof(_SampleView),
                    Order = 10
                }
            */
        };

        public ObservableRangeCollection<NavigationMenuItem> GetAllMenuItems()
        {
            return _menuItems;
        }

        public ObservableRangeCollection<NavigationMenuItem> GetAuthorizedMenuItems(Dictionary<string, string> grantedPermissions)
        {
            var authorizedMenuItems = new ObservableRangeCollection<NavigationMenuItem>();
            foreach (var menuItem in _menuItems)
            {
                if (menuItem.RequiredPermissionName == null)
                {
                    authorizedMenuItems.Add(menuItem);
                    continue;
                }

                if (grantedPermissions != null &&
                    grantedPermissions.ContainsKey(menuItem.RequiredPermissionName))
                {
                    authorizedMenuItems.Add(menuItem);
                }
            }

            return authorizedMenuItems;
        }
    }
}