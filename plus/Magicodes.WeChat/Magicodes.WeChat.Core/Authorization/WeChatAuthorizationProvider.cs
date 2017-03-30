using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Magicodes.WeChat.Core.Authorization
{
    /// <summary>
    /// 应用程序权限提供程序
    /// 定义应用程序权限
    /// 访问 <see cref="WeChatAuthorizationProvider"/> 可以查看所以权限定义
    /// </summary>
    public class WeChatAuthorizationProvider : AuthorizationProvider
    {
        private readonly bool _isMultiTenancyEnabled;

        public WeChatAuthorizationProvider(bool isMultiTenancyEnabled)
        {
            _isMultiTenancyEnabled = isMultiTenancyEnabled;
        }

        public WeChatAuthorizationProvider(IMultiTenancyConfig multiTenancyConfig)
        {
            _isMultiTenancyEnabled = multiTenancyConfig.IsEnabled;
        }

        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //权限定义
            //定义页面权限节点
            var pagesPer = WeChatPermissions.CreatePermissions().Pages();
            var pages = context.GetPermissionOrNull(pagesPer.ToString()) ?? context.CreatePermission(pagesPer.ToString(), L("Pages"), multiTenancySides: MultiTenancySides.Tenant);

            var tenantPer = pagesPer.Tenants();
            #region 定义粉丝管理权限
            var weChatUsersPer = tenantPer.Clone().WeChatUsers();
            //创建基于租户的增删改查权限
            CreateCRUDPermission(weChatUsersPer, pages);
            #endregion

            #region 定义接口设置权限
            var weChatApiSettingPer = tenantPer.Clone().WeChatApiSetting();
            //创建基于租户的增删改查权限
            CreateCRUDPermission(weChatApiSettingPer, pages, get: false);
            #endregion
        }

        private static void CreateCRUDPermission(WeChatPermissions PerString, Permission permissionper, bool create = true, bool update = true, bool delete = true, bool get = true)
        {
            if (get)
            {
                var indexPermissionName = PerString.ToString();
                permissionper = permissionper.CreateChildPermission(indexPermissionName, L(indexPermissionName.Replace('.', '_')), multiTenancySides: MultiTenancySides.Tenant);
            }
            if (create)
            {
                var createPermissionName = PerString.Clone().Create().ToString();
                permissionper.CreateChildPermission(createPermissionName, L("Create"), multiTenancySides: MultiTenancySides.Tenant);
            }
            if (update)
            {
                var editPermissionName = PerString.Clone().Edit().ToString();
                permissionper.CreateChildPermission(editPermissionName, L("Edit"), multiTenancySides: MultiTenancySides.Tenant);
            }
            if (delete)
            {
                var detelePermissionName = PerString.Clone().Delete().ToString();
                permissionper.CreateChildPermission(detelePermissionName, L("Delete"), multiTenancySides: MultiTenancySides.Tenant);
            }
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, WeChatConsts.LocalizationSourceName);
        }
    }
}
