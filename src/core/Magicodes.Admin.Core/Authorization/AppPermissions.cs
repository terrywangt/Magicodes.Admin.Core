namespace Magicodes.Admin.Authorization
{
    /// <summary>
    /// Defines string constants for application's permission names.
    /// <see cref="AppAuthorizationProvider"/> for permission definitions.
    /// </summary>
    public static partial class AppPermissions
    {
        //COMMON PERMISSIONS (FOR BOTH OF TENANTS AND HOST)

        public const string Pages = "Pages";

        public const string Pages_DemoUiComponents= "Pages.DemoUiComponents";
        public const string Pages_Administration = "Pages.Administration";

        public const string Pages_Administration_Roles = "Pages.Administration.Roles";
        public const string Pages_Administration_Roles_Create = "Pages.Administration.Roles.Create";
        public const string Pages_Administration_Roles_Edit = "Pages.Administration.Roles.Edit";
        public const string Pages_Administration_Roles_Delete = "Pages.Administration.Roles.Delete";

        public const string Pages_Administration_Users = "Pages.Administration.Users";
        public const string Pages_Administration_Users_Create = "Pages.Administration.Users.Create";
        public const string Pages_Administration_Users_Edit = "Pages.Administration.Users.Edit";
        public const string Pages_Administration_Users_Delete = "Pages.Administration.Users.Delete";
        public const string Pages_Administration_Users_ChangePermissions = "Pages.Administration.Users.ChangePermissions";
        public const string Pages_Administration_Users_Impersonation = "Pages.Administration.Users.Impersonation";

        public const string Pages_Administration_Languages = "Pages.Administration.Languages";
        public const string Pages_Administration_Languages_Create = "Pages.Administration.Languages.Create";
        public const string Pages_Administration_Languages_Edit = "Pages.Administration.Languages.Edit";
        public const string Pages_Administration_Languages_Delete = "Pages.Administration.Languages.Delete";
        public const string Pages_Administration_Languages_ChangeTexts = "Pages.Administration.Languages.ChangeTexts";

        public const string Pages_Administration_AuditLogs = "Pages.Administration.AuditLogs";

        public const string Pages_Administration_OrganizationUnits = "Pages.Administration.OrganizationUnits";
        public const string Pages_Administration_OrganizationUnits_ManageOrganizationTree = "Pages.Administration.OrganizationUnits.ManageOrganizationTree";
        public const string Pages_Administration_OrganizationUnits_ManageMembers = "Pages.Administration.OrganizationUnits.ManageMembers";

        public const string Pages_Administration_HangfireDashboard = "Pages.Administration.HangfireDashboard";

        public const string Pages_Administration_UiCustomization = "Pages.Administration.UiCustomization";

        public const string Pages_Administration_Pay_Settings = "Pages.Administration.Pay.Settings";

        public const string Pages_Administration_SmsCode_Settings = "Pages.Administration.SmsCode.Settings";

        public const string Pages_Administration_Storage_Settings = "Pages.Administration.Storage.Settings";

        //TENANT-SPECIFIC PERMISSIONS

        public const string Pages_Tenant_Dashboard = "Pages.Tenant.Dashboard";

        public const string Pages_Administration_Tenant_Settings = "Pages.Administration.Tenant.Settings";

        public const string Pages_Administration_Tenant_SubscriptionManagement = "Pages.Administration.Tenant.SubscriptionManagement";

        //HOST-SPECIFIC PERMISSIONS

        public const string Pages_Editions = "Pages.Editions";
        public const string Pages_Editions_Create = "Pages.Editions.Create";
        public const string Pages_Editions_Edit = "Pages.Editions.Edit";
        public const string Pages_Editions_Delete = "Pages.Editions.Delete";

        public const string Pages_Tenants = "Pages.Tenants";
        public const string Pages_Tenants_Create = "Pages.Tenants.Create";
        public const string Pages_Tenants_Edit = "Pages.Tenants.Edit";
        public const string Pages_Tenants_ChangeFeatures = "Pages.Tenants.ChangeFeatures";
        public const string Pages_Tenants_Delete = "Pages.Tenants.Delete";
        public const string Pages_Tenants_Impersonation = "Pages.Tenants.Impersonation";

        public const string Pages_Administration_Host_Maintenance = "Pages.Administration.Host.Maintenance";
        public const string Pages_Administration_Host_Settings = "Pages.Administration.Host.Settings";
        public const string Pages_Administration_Host_Dashboard = "Pages.Administration.Host.Dashboard";


        #region【小程序配置】
        public const string Pages_Administration_MiniProgram_Settings = "Pages.Administration.MiniProgram.Settings";
        #endregion

        #region TransactionLog【交易日志】

        public const string Pages_TransactionLog = "Pages.TransactionLog";

        //public const string Pages_TransactionLog_Create = "Pages.TransactionLog.Create";

        public const string Pages_TransactionLog_Edit = "Pages.TransactionLog.Edit";

        public const string Pages_TransactionLog_Delete = "Pages.TransactionLog.Delete";

        public const string Pages_TransactionLog_Restore = "Pages.TransactionLog.Restore";

        #endregion

        #region ArticleTagInfos【文章标签】

        public const string Pages_ArticleInfo_ArticleTagInfo = "Pages.ArticleInfo.ArticleTagInfo";

        public const string Pages_ArticleInfo_ArticleTagInfo_Create = "Pages_ArticleInfo_ArticleTagInfo.Create";

        public const string Pages_ArticleInfo_ArticleTagInfo_Edit = "Pages_ArticleInfo_ArticleTagInfo.Edit";

        public const string Pages_ArticleInfo_ArticleTagInfo_Delete = "Pages_ArticleInfo_ArticleTagInfo.Delete";

        public const string Pages_ArticleInfo_ArticleTagInfo_Restore = "Pages_ArticleInfo_ArticleTagInfo.Restore";

        #endregion

        #region ArticleInfo【文章】

        public const string Pages_ArticleInfo = "Pages.ArticleInfo";

        public const string Pages_ArticleInfo_Create = "Pages.ArticleInfo.Create";

        public const string Pages_ArticleInfo_Edit = "Pages.ArticleInfo.Edit";

        public const string Pages_ArticleInfo_Delete = "Pages.ArticleInfo.Delete";

        public const string Pages_ArticleInfo_Restore = "Pages.ArticleInfo.Restore";

        #endregion

        #region ArticleSourceInfo【文章来源】

        public const string Pages_ArticleSourceInfo = "Pages.ArticleSourceInfo";

        public const string Pages_ArticleSourceInfo_Create = "Pages.ArticleSourceInfo.Create";

        public const string Pages_ArticleSourceInfo_Edit = "Pages.ArticleSourceInfo.Edit";

        public const string Pages_ArticleSourceInfo_Delete = "Pages.ArticleSourceInfo.Delete";

        public const string Pages_ArticleSourceInfo_Restore = "Pages.ArticleSourceInfo.Restore";

        #endregion

        #region ColumnInfo【栏目】

        public const string Pages_ColumnInfo = "Pages.ColumnInfo";

        public const string Pages_ColumnInfo_Create = "Pages.ColumnInfo.Create";

        public const string Pages_ColumnInfo_Edit = "Pages.ColumnInfo.Edit";

        public const string Pages_ColumnInfo_Delete = "Pages.ColumnInfo.Delete";

        public const string Pages_ColumnInfo_Restore = "Pages.ColumnInfo.Restore";

        #endregion
    }
}