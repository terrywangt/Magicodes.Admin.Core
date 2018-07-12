using System.Threading.Tasks;
using Abp.Configuration;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.Configuration.Tenants;
using Shouldly;
using Xunit;

namespace Magicodes.Admin.Tests.Configuration.Tenants
{
    public class TenantSettingsAppService_Tests : AppTestBase
    {
        private readonly ITenantSettingsAppService _tenantSettingsAppService;
        private readonly ISettingManager _settingManager;

        public TenantSettingsAppService_Tests()
        {
            _tenantSettingsAppService = Resolve<ITenantSettingsAppService>();
            _settingManager = Resolve<ISettingManager>();

            LoginAsDefaultTenantAdmin();
            InitializeTestSettings();
        }

        private void InitializeTestSettings()
        {
            _settingManager.ChangeSettingForApplicationAsync(AppSettings.UserManagement.AllowSelfRegistration, "true");
            _settingManager.ChangeSettingForApplicationAsync(AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault, "false");
        }

        [Fact(Skip = "Getting exception: Abp.Authorization.AbpAuthorizationException : Required permissions are not granted. At least one of these permissions must be granted: Settings")]
        public async Task Should_Change_UserManagement_Settings()
        {
            //Get and check current settings

            //Act
            var settings = await _tenantSettingsAppService.GetAllSettings();

            //Assert
            settings.UserManagement.AllowSelfRegistration.ShouldBe(true);
            settings.UserManagement.IsNewRegisteredUserActiveByDefault.ShouldBe(false);
            settings.UserManagement.UseCaptchaOnRegistration.ShouldBe(true);

            //Change and save settings

            //Arrange
            settings.UserManagement.AllowSelfRegistration = true;
            settings.UserManagement.IsNewRegisteredUserActiveByDefault = true;
            settings.UserManagement.UseCaptchaOnRegistration = false;

            await _tenantSettingsAppService.UpdateAllSettings(settings);

            //Assert
            _settingManager.GetSettingValue<bool>(AppSettings.UserManagement.AllowSelfRegistration).ShouldBe(true);
            _settingManager.GetSettingValue<bool>(AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault).ShouldBe(true);
            _settingManager.GetSettingValue<bool>(AppSettings.UserManagement.UseCaptchaOnRegistration).ShouldBe(false);
        }
    }
}
