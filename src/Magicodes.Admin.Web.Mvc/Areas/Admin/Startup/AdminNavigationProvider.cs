using Abp.Application.Navigation;
using Abp.Localization;
using Magicodes.Admin.Authorization;

namespace Magicodes.Admin.Web.Areas.Admin.Startup
{
    public class AdminNavigationProvider : NavigationProvider
    {
        public const string MenuName = "App";

        public override void SetNavigation(INavigationProviderContext context)
        {
            var menu = context.Manager.Menus[MenuName] = new MenuDefinition(MenuName, new FixedLocalizableString("Main Menu"));

            menu
                .AddItem(new MenuItemDefinition(
                        AdminPageNames.Host.Dashboard,
                        L("Dashboard"),
                        url: "Admin/HostDashboard",
                        icon: "flaticon-line-graph",
                        requiredPermissionName: AppPermissions.Pages_Administration_Host_Dashboard
                    )
                ).AddItem(new MenuItemDefinition(
                    AdminPageNames.Host.Tenants,
                    L("Tenants"),
                    url: "Admin/Tenants",
                    icon: "flaticon-list-3",
                    requiredPermissionName: AppPermissions.Pages_Tenants
                    )
                ).AddItem(new MenuItemDefinition(
                        AdminPageNames.Host.Editions,
                        L("Editions"),
                        url: "Admin/Editions",
                        icon: "flaticon-app",
                        requiredPermissionName: AppPermissions.Pages_Editions
                    )
                ).AddItem(new MenuItemDefinition(
                        AdminPageNames.Tenant.Dashboard,
                        L("Dashboard"),
                        url: "Admin/Dashboard",
                        icon: "flaticon-line-graph",
                        requiredPermissionName: AppPermissions.Pages_Tenant_Dashboard
                    )
                ).AddItem(new MenuItemDefinition(
                        AdminPageNames.Common.Administration,
                        L("Administration"),
                        icon: "flaticon-interface-8"
                    ).AddItem(new MenuItemDefinition(
                            AdminPageNames.Common.OrganizationUnits,
                            L("OrganizationUnits"),
                            url: "Admin/OrganizationUnits",
                            icon: "flaticon-map",
                            requiredPermissionName: AppPermissions.Pages_Administration_OrganizationUnits
                        )
                    ).AddItem(new MenuItemDefinition(
                            AdminPageNames.Common.Roles,
                            L("Roles"),
                            url: "Admin/Roles",
                            icon: "flaticon-suitcase",
                            requiredPermissionName: AppPermissions.Pages_Administration_Roles
                        )
                    ).AddItem(new MenuItemDefinition(
                            AdminPageNames.Common.Users,
                            L("Users"),
                            url: "Admin/Users",
                            icon: "flaticon-users",
                            requiredPermissionName: AppPermissions.Pages_Administration_Users
                        )
                    ).AddItem(new MenuItemDefinition(
                            AdminPageNames.Common.Languages,
                            L("Languages"),
                            url: "Admin/Languages",
                            icon: "flaticon-tabs",
                            requiredPermissionName: AppPermissions.Pages_Administration_Languages
                        )
                    ).AddItem(new MenuItemDefinition(
                            AdminPageNames.Common.AuditLogs,
                            L("AuditLogs"),
                            url: "Admin/AuditLogs",
                            icon: "flaticon-folder-1",
                            requiredPermissionName: AppPermissions.Pages_Administration_AuditLogs
                        )
                    ).AddItem(new MenuItemDefinition(
                            AdminPageNames.Host.Maintenance,
                            L("Maintenance"),
                            url: "Admin/Maintenance",
                            icon: "flaticon-lock",
                            requiredPermissionName: AppPermissions.Pages_Administration_Host_Maintenance
                        )
                    ).AddItem(new MenuItemDefinition(
                            AdminPageNames.Tenant.SubscriptionManagement,
                            L("Subscription"),
                            url: "Admin/SubscriptionManagement",
                            icon: "flaticon-refresh"
                            ,
                            requiredPermissionName: AppPermissions.Pages_Administration_Tenant_SubscriptionManagement
                        )
                    ).AddItem(new MenuItemDefinition(
                            AdminPageNames.Common.UiCustomization,
                            L("VisualSettings"),
                            url: "Admin/UiCustomization",
                            icon: "flaticon-medical",
                            requiredPermissionName: AppPermissions.Pages_Administration_UiCustomization
                        )
                    ).AddItem(new MenuItemDefinition(
                            AdminPageNames.Host.Settings,
                            L("Settings"),
                            url: "Admin/HostSettings",
                            icon: "flaticon-settings",
                            requiredPermissionName: AppPermissions.Pages_Administration_Host_Settings
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                            AdminPageNames.Tenant.Settings,
                            L("Settings"),
                            url: "Admin/Settings",
                            icon: "flaticon-settings",
                            requiredPermissionName: AppPermissions.Pages_Administration_Tenant_Settings
                        )
                    )
                ).AddItem(new MenuItemDefinition(
                        AdminPageNames.Common.DemoUiComponents,
                        L("DemoUiComponents"),
                        url: "Admin/DemoUiComponents",
                        icon: "flaticon-shapes",
                        requiredPermissionName: AppPermissions.Pages_DemoUiComponents
                    )
                );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, AdminConsts.LocalizationSourceName);
        }
    }
}