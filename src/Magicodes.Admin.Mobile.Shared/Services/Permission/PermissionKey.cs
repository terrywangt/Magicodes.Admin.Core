namespace Magicodes.Admin.Services.Permission
{
    public class PermissionKey
    {
        public const string Pages = "Pages";
        public const string DemoUiComponents = "Pages.DemoUiComponents";
        public const string Administration = "Pages.Administration";

        /* HOST DASHBOARD */
        public const string HostDashboard = "Pages.Administration.Host.Dashboard";

        /* TENANTS */
        public const string Tenants = "Pages.Tenants";
        public const string TenantDashboard = "Pages.Tenant.Dashboard";
        public const string TenantCreate = "Pages.Tenants.Create";
        public const string TenantEdit = "Pages.Tenants.Edit";
        public const string TenantChangeFeatures = "Pages.Tenants.ChangeFeatures";
        public const string TenantDelete = "Pages.Tenants.Delete";
        public const string TenantImpersonation = "Pages.Tenants.Impersonation";
        public const string TenantSettings = "Pages.Administration.Tenant.Settings";

        /* EDITIONS */
        public const string Editions = "Pages.Editions";
        public const string EditionCreate = "Pages.Editions.Create";
        public const string EditionEdit = "Pages.Editions.Edit";
        public const string EditionDelete = "Pages.Editions.Delete";
        public const string TenantSubscriptionManagement = "Pages.Administration.Tenant.SubscriptionManagement";

        /* ADMINISTRATION > ORGANIZATION UNIT */
        public const string OrganizationUnits = "Pages.Administration.OrganizationUnits";
        public const string OrganizationUnitManageOrganizationTree = "Pages.Administration.OrganizationUnits.ManageOrganizationTree";
        public const string OrganizationUnitManageMembers = "Pages.Administration.OrganizationUnits.ManageMembers";

        /* ADMINISTRATION > ROLES */
        public const string Roles = "Pages.Administration.Roles";
        public const string RoleCreate = "Pages.Administration.Roles.Create";
        public const string RoleEdit = "Pages.Administration.Roles.Edit";
        public const string RoleDelete = "Pages.Administration.Roles.Delete";

        /* ADMINISTRATION > USERS */
        public const string Users = "Pages.Administration.Users";
        public const string UserCreate = "Pages.Administration.Users.Create";
        public const string UserEdit = "Pages.Administration.Users.Edit";
        public const string UserDelete = "Pages.Administration.Users.Delete";
        public const string UserChangePermission = "Pages.Administration.Users.ChangePermissions";
        public const string UserImpersonation = "Pages.Administration.Users.Impersonation";

        /* ADMINISTRATION - LANGUAGES */
        public const string Languages = "Pages.Administration.Languages";
        public const string LanguageCreate = "Pages.Administration.Languages.Create";
        public const string LanguageEdit = "Pages.Administration.Languages.Edit";
        public const string LanguageDelete = "Pages.Administration.Languages.Delete";
        public const string LanguageChangeTexts = "Pages.Administration.Languages.ChangeTexts";

        /* ADMINISTRATION > AUDIT LOGS */
        public const string AuditLogs = "Pages.Administration.AuditLogs";
      
        /* ADMINISTRATION > MAITENANCE */
        public const string HostMaintenance = "Pages.Administration.Host.Maintenance";

        /* ADMINISTRATION > MAITENANCE */
        public const string HostSettings = "Pages.Administration.Host.Settings";

        /* ADMINISTRATION > HANGFIRE DASHBOARD */
        public const string HangfireDashboard = "Pages.Administration.HangfireDashboard";
    }
}