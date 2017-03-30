using System;

namespace Magicodes.WeChat.Core.Authorization
{
    /// <summary>
    /// 定义应用程序权限名称常量
    /// <see cref="WeChatAuthorizationProvider"/> 权限定义.
    /// </summary>
    public class WeChatPermissions
    {
        public const string WeChatPermissions_Pages = "WeChatPermissions.Pages";

        public const string WeChatPermissions_Pages_Tenants = "WeChatPermissions.Pages.Tenants";

        public const string WeChatPermissions_Pages_Tenants_WeChatUsers = "WeChatPermissions.Pages.Tenants.WeChatUsers";

        public const string WeChatPermissions_Pages_Tenants_WeChatUsers_Create = "WeChatPermissions.Pages.Tenants.WeChatUsers.Create";

        public const string WeChatPermissions_Pages_Tenants_WeChatUsers_Edit = "WeChatPermissions.Pages.Tenants.WeChatUsers.Edit";

        public const string WeChatPermissions_Pages_Tenants_WeChatUsers_Delete = "WeChatPermissions.Pages.Tenants.WeChatUsers.Delete";

        public const string WeChatPermissions_Pages_Tenants_WeChatApiSetting = "WeChatPermissions.Pages.Tenants.WeChatApiSetting";

        string PermissionsStr { get; set; }
        #region 方法
        public WeChatPermissions()
        {
            PermissionsStr = "WeChatPermissions";
        }

        public WeChatPermissions(string permissionsStr)
        {
            PermissionsStr = permissionsStr;
        }

        public static WeChatPermissions CreatePermissions()
        {
            return new WeChatPermissions();
        }

        public WeChatPermissions Pages()
        {
            PermissionsStr += ".Pages";
            return this;
        }

        public WeChatPermissions Administration()
        {
            PermissionsStr += ".Administration";
            return this;
        }

        public WeChatPermissions Tenants()
        {
            PermissionsStr += ".Tenants";
            return this;
        }

        public WeChatPermissions WeChatUsers()
        {
            PermissionsStr += ".WeChatUsers";
            return this;
        }

        public WeChatPermissions WeChatApiSetting()
        {
            PermissionsStr += ".WeChatApiSetting";
            return this;
        }

        public WeChatPermissions Create()
        {
            PermissionsStr += ".Create";
            return this;
        }

        public WeChatPermissions Edit()
        {
            PermissionsStr += ".Edit";
            return this;
        }

        public WeChatPermissions Delete()
        {
            PermissionsStr += ".Delete";
            return this;
        }

        public override string ToString()
        {
            return this.PermissionsStr;
        }

        public WeChatPermissions Clone()
        {
            return new WeChatPermissions(PermissionsStr);
        } 
        #endregion
    }
}