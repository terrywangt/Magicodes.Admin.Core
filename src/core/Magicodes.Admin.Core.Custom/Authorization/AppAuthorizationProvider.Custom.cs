using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;

namespace Magicodes.Admin.Core.Custom.Authorization
{
    /// <summary>
    /// Application's authorization provider.
    /// Defines permissions for the application.
    /// See <see cref="AppCustomPermissions"/> for all permission names.
    /// </summary>
    public partial class AppCustomAuthorizationProvider : AuthorizationProvider
    {
        private readonly bool _isMultiTenancyEnabled;

        public AppCustomAuthorizationProvider(bool isMultiTenancyEnabled)
        {
            _isMultiTenancyEnabled = isMultiTenancyEnabled;
        }

        public AppCustomAuthorizationProvider(IMultiTenancyConfig multiTenancyConfig)
        {
            _isMultiTenancyEnabled = multiTenancyConfig.IsEnabled;
        }

        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //TODO：用户自定义
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, AdminConsts.LocalizationSourceName);
        }
    }
}