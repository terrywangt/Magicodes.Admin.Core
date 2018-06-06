using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Magicodes.Admin.Authorization
{
    /// <summary>
    /// 应用程序权限提供程序
    /// </summary>
    public partial class AppAuthorizationProvider
    {

        /// <summary>
        /// 设置权限
        /// </summary>
        /// <param name="context"></param>
        public void SetCustomPermissions(Permission root)
        {
            //TODO：自定义权限设置（自动生成并覆盖）
        }
    }
}