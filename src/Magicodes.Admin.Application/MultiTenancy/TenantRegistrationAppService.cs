using System;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.UI;
using Abp.Zero.Configuration;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.Debugging;
using Magicodes.Admin.Editions;
using Magicodes.Admin.MultiTenancy.Dto;
using Magicodes.Admin.Notifications;
using Magicodes.Admin.Security.Recaptcha;
using Magicodes.Admin.Url;

namespace Magicodes.Admin.MultiTenancy
{
    public class TenantRegistrationAppService : AdminAppServiceBase, ITenantRegistrationAppService
    {
        public IAppUrlService AppUrlService { get; set; }

        private readonly IMultiTenancyConfig _multiTenancyConfig;
        private readonly IRecaptchaValidator _recaptchaValidator;
        private readonly EditionManager _editionManager;
        private readonly IAppNotifier _appNotifier;

        public TenantRegistrationAppService(
            IMultiTenancyConfig multiTenancyConfig, 
            IRecaptchaValidator recaptchaValidator, 
            EditionManager editionManager, 
            IAppNotifier appNotifier)
        {
            _multiTenancyConfig = multiTenancyConfig;
            _recaptchaValidator = recaptchaValidator;
            _editionManager = editionManager;
            _appNotifier = appNotifier;

            AppUrlService = NullAppUrlService.Instance;
        }

        public async Task<RegisterTenantOutput> RegisterTenant(RegisterTenantInput input)
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                CheckTenantRegistrationIsEnabled();

                if (UseCaptchaOnRegistration())
                {
                    await _recaptchaValidator.ValidateAsync(input.CaptchaResponse);
                }

                //Getting host-specific settings
                var isNewRegisteredTenantActiveByDefault = await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.TenantManagement.IsNewRegisteredTenantActiveByDefault);
                var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueForApplicationAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);
                var defaultEditionIdValue = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.TenantManagement.DefaultEdition);
                int? defaultEditionId = null;

                if (!string.IsNullOrEmpty(defaultEditionIdValue) && (await _editionManager.FindByIdAsync(Convert.ToInt32(defaultEditionIdValue)) != null))
                {
                    defaultEditionId = Convert.ToInt32(defaultEditionIdValue);
                }

                var tenantId = await TenantManager.CreateWithAdminUserAsync(
                    input.TenancyName,
                    input.Name,
                    input.AdminPassword,
                    input.AdminEmailAddress,
                    null,
                    isNewRegisteredTenantActiveByDefault,
                    defaultEditionId,
                    false,
                    true,
                    AppUrlService.CreateEmailActivationUrlFormat(input.TenancyName)
                );

                var tenant = await TenantManager.GetByIdAsync(tenantId);
                await _appNotifier.NewTenantRegisteredAsync(tenant);

                return new RegisterTenantOutput
                {
                    TenantId = tenantId,
                    TenancyName = input.TenancyName,
                    Name = input.Name,
                    UserName = Authorization.Users.User.AdminUserName,
                    EmailAddress = input.AdminEmailAddress,
                    IsActive = isNewRegisteredTenantActiveByDefault,
                    IsEmailConfirmationRequired = isEmailConfirmationRequiredForLogin,
                    IsTenantActive = tenant.IsActive
                };
            }
        }

        private void CheckTenantRegistrationIsEnabled()
        {
            if (!IsSelfRegistrationEnabled())
            {
                throw new UserFriendlyException(L("SelfTenantRegistrationIsDisabledMessage_Detail"));
            }

            if (!_multiTenancyConfig.IsEnabled)
            {
                throw new UserFriendlyException(L("MultiTenancyIsNotEnabled"));
            }
        }

        private bool IsSelfRegistrationEnabled()
        {
            return SettingManager.GetSettingValueForApplication<bool>(AppSettings.TenantManagement.AllowSelfRegistration);
        }

        private bool UseCaptchaOnRegistration()
        {
            if (DebugHelper.IsDebug)
            {
                return false;
            }

            return SettingManager.GetSettingValueForApplication<bool>(AppSettings.TenantManagement.UseCaptchaOnRegistration);
        }
    }
}
