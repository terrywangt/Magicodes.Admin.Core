using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Magicodes.Admin.ApiClient;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.Core.Dependency;
using Magicodes.Admin.Core.Threading;
using Magicodes.Admin.Localization;
using Magicodes.Admin.Localization.Resources;
using Magicodes.Admin.UI;

namespace Magicodes.Admin.ViewModels.Base
{
    public static class UserConfigurationManager
    {
        private static readonly Lazy<IApplicationContext> AppContext = new Lazy<IApplicationContext>(
            DependencyResolver.Resolve<IApplicationContext>
        );

        private static IAccessTokenManager AccessTokenManager => DependencyResolver.IocManager.Resolve<IAccessTokenManager>();

        public static async Task GetIfNeedsAsync()
        {
            if (AppContext.Value.Configuration != null)
            {
                return;
            }

            await GetAsync();
        }

        public static async Task GetAsync()
        {
            var userConfigurationService = DependencyResolver.IocManager.Resolve<UserConfigurationService>();

            await WebRequestExecuter.Execute(
                async () => await userConfigurationService.GetAsync(AccessTokenManager.IsUserLoggedIn),
                result =>
                {
                    AppContext.Value.Configuration = result;
                    SetCurrentCulture();
                    WarnIfUserHasNoPermission();
                    return Task.CompletedTask;
                },
                _ =>
                {
                    App.ExitApplication();
                    return Task.CompletedTask;
                },
                () => DependencyResolver
                    .IocManager
                    .Release(userConfigurationService)
            );
        }

        private static void WarnIfUserHasNoPermission()
        {
            if (!AccessTokenManager.IsUserLoggedIn)
            {
                return;
            }

            var hasAnyPermission = AppContext.Value.Configuration.Auth.GrantedPermissions != null &&
                                   AppContext.Value.Configuration.Auth.GrantedPermissions.Any();

            if (!hasAnyPermission)
            {
                UserDialogHelper.Warn("NoPermission");
            }
        }

        private static void SetCurrentCulture()
        {
            var locale = DependencyResolver.Resolve<ILocale>();
            var userCulture = GetUserCulture(locale);

            locale.SetLocale(userCulture);
            LocalTranslation.Culture = userCulture;
        }

        private static CultureInfo GetUserCulture(ILocale locale)
        {
            if (AppContext.Value.Configuration.Localization.CurrentCulture.Name == null)
            {
                return locale.GetCurrentCultureInfo();
            }

            try
            {
                return new CultureInfo(AppContext.Value.Configuration.Localization.CurrentCulture.Name);
            }
            catch (CultureNotFoundException)
            {
                return locale.GetCurrentCultureInfo();
            }
        }

    }
}