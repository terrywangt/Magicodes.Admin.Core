using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Configuration;
using Abp.Zero.Configuration;
using Microsoft.Extensions.Configuration;

namespace Magicodes.Admin.Configuration
{
    /// <summary>
    /// Defines settings for the application.
    /// See <see cref="AppSettings"/> for setting names.
    /// </summary>
    public class AppSettingProvider : SettingProvider
    {
        private readonly IConfigurationRoot _appConfiguration;

        public AppSettingProvider(IAppConfigurationAccessor configurationAccessor)
        {
            _appConfiguration = configurationAccessor.Configuration;
        }

        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            //Disable TwoFactorLogin by default (can be enabled by UI)
            context.Manager.GetSettingDefinition(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled).DefaultValue = false.ToString().ToLowerInvariant();

            return GetHostSettings().Union(GetTenantSettings()).Union(GetSharedSettings()).Union(GetPaySettings())
                .Union(GetSmsCodeSettings());
        }

        private IEnumerable<SettingDefinition> GetHostSettings()
        {
            return new[] {
                new SettingDefinition(AppSettings.TenantManagement.AllowSelfRegistration, GetFromAppSettings(AppSettings.TenantManagement.AllowSelfRegistration, "true"), isVisibleToClients: true),
                new SettingDefinition(AppSettings.TenantManagement.IsNewRegisteredTenantActiveByDefault, GetFromAppSettings(AppSettings.TenantManagement.IsNewRegisteredTenantActiveByDefault, "false")),
                new SettingDefinition(AppSettings.TenantManagement.UseCaptchaOnRegistration, GetFromAppSettings(AppSettings.TenantManagement.UseCaptchaOnRegistration, "true"), isVisibleToClients: true),
                new SettingDefinition(AppSettings.TenantManagement.DefaultEdition, GetFromAppSettings(AppSettings.TenantManagement.DefaultEdition, "")),
                new SettingDefinition(AppSettings.UserManagement.SmsVerificationEnabled, GetFromAppSettings(AppSettings.UserManagement.SmsVerificationEnabled, "false"), isVisibleToClients: true),
                new SettingDefinition(AppSettings.TenantManagement.SubscriptionExpireNotifyDayCount, GetFromAppSettings(AppSettings.TenantManagement.SubscriptionExpireNotifyDayCount, "7"), isVisibleToClients: true),
                new SettingDefinition(AppSettings.HostManagement.BillingLegalName, GetFromAppSettings(AppSettings.HostManagement.BillingLegalName, "")),
                new SettingDefinition(AppSettings.HostManagement.BillingAddress, GetFromAppSettings(AppSettings.HostManagement.BillingAddress, "")),
                new SettingDefinition(AppSettings.HostManagement.BillingTaxNumber, GetFromAppSettings(AppSettings.HostManagement.BillingTaxNumber, "")),
                new SettingDefinition(AppSettings.HostManagement.BillingContact, GetFromAppSettings(AppSettings.HostManagement.BillingContact, "")),
                new SettingDefinition(AppSettings.HostManagement.BillingBankAccount, GetFromAppSettings(AppSettings.HostManagement.BillingBankAccount, "")),
                new SettingDefinition(AppSettings.HostManagement.BillingBank, GetFromAppSettings(AppSettings.HostManagement.BillingBank, "")),

                new SettingDefinition(AppSettings.Recaptcha.SiteKey, GetFromSettings("Recaptcha:SiteKey"), isVisibleToClients: true),

                //UI customization options
                new SettingDefinition(AppSettings.UiManagement.LayoutType, GetFromAppSettings(AppSettings.UiManagement.LayoutType, "fluid"), isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(AppSettings.UiManagement.ContentSkin, GetFromAppSettings(AppSettings.UiManagement.ContentSkin, "light2"), isVisibleToClients: true, scopes: SettingScopes.All),

                new SettingDefinition(AppSettings.UiManagement.Header.DesktopFixedHeader, GetFromAppSettings(AppSettings.UiManagement.Header.DesktopFixedHeader, "true"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(AppSettings.UiManagement.Header.DesktopMinimizeMode, GetFromAppSettings(AppSettings.UiManagement.Header.DesktopMinimizeMode, ""),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(AppSettings.UiManagement.Header.MobileFixedHeader, GetFromAppSettings(AppSettings.UiManagement.Header.MobileFixedHeader, "false"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(AppSettings.UiManagement.Header.Skin, GetFromAppSettings(AppSettings.UiManagement.Header.Skin, "light"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(AppSettings.UiManagement.Header.DisplaySubmenuArrowDesktop, GetFromAppSettings(AppSettings.UiManagement.Header.DisplaySubmenuArrowDesktop, "true"),isVisibleToClients: true, scopes: SettingScopes.All),

                new SettingDefinition(AppSettings.UiManagement.LeftAside.Position, GetFromAppSettings(AppSettings.UiManagement.LeftAside.Position, "left"), isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(AppSettings.UiManagement.LeftAside.AsideSkin, GetFromAppSettings(AppSettings.UiManagement.LeftAside.AsideSkin, "light"), isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(AppSettings.UiManagement.LeftAside.FixedAside, GetFromAppSettings(AppSettings.UiManagement.LeftAside.FixedAside, "true"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(AppSettings.UiManagement.LeftAside.AllowAsideMinimizing, GetFromAppSettings(AppSettings.UiManagement.LeftAside.AllowAsideMinimizing, "true"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(AppSettings.UiManagement.LeftAside.DefaultMinimizedAside, GetFromAppSettings(AppSettings.UiManagement.LeftAside.DefaultMinimizedAside, "false"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(AppSettings.UiManagement.LeftAside.AllowAsideHiding, GetFromAppSettings(AppSettings.UiManagement.LeftAside.AllowAsideHiding, "true"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(AppSettings.UiManagement.LeftAside.DefaultHiddenAside, GetFromAppSettings(AppSettings.UiManagement.LeftAside.DefaultHiddenAside, "false"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(AppSettings.UiManagement.LeftAside.SubmenuToggle, GetFromAppSettings(AppSettings.UiManagement.LeftAside.SubmenuToggle, "accordion"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(AppSettings.UiManagement.LeftAside.DropdownSubmenuSkin, GetFromAppSettings(AppSettings.UiManagement.LeftAside.DropdownSubmenuSkin, "inherit"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(AppSettings.UiManagement.LeftAside.DropdownSubmenuArrow, GetFromAppSettings(AppSettings.UiManagement.LeftAside.DropdownSubmenuArrow, "true"),isVisibleToClients: true, scopes: SettingScopes.All),

                new SettingDefinition(AppSettings.UiManagement.Footer.FixedFooter, GetFromAppSettings(AppSettings.UiManagement.Footer.FixedFooter, "false"),isVisibleToClients: true, scopes: SettingScopes.All),

                new SettingDefinition(AppSettings.UiManagement.Theme, GetFromAppSettings(AppSettings.UiManagement.Theme, "default"),isVisibleToClients: true, scopes: SettingScopes.All)
            };
        }

        private IEnumerable<SettingDefinition> GetPaySettings()
        {
            return new[] {
                new SettingDefinition(AppSettings.WeChatPayManagement.AppId, GetFromAppSettings(AppSettings.WeChatPayManagement.AppId, ""),scopes: SettingScopes.Tenant|SettingScopes.Application),
                new SettingDefinition(AppSettings.WeChatPayManagement.MchId, GetFromAppSettings(AppSettings.WeChatPayManagement.MchId, ""),scopes: SettingScopes.Tenant|SettingScopes.Application),
                new SettingDefinition(AppSettings.WeChatPayManagement.TenPayKey, GetFromAppSettings(AppSettings.WeChatPayManagement.TenPayKey, ""),scopes: SettingScopes.Tenant|SettingScopes.Application),
                new SettingDefinition(AppSettings.WeChatPayManagement.PayNotifyUrl, GetFromAppSettings(AppSettings.WeChatPayManagement.PayNotifyUrl, ""),scopes: SettingScopes.Tenant|SettingScopes.Application),
                new SettingDefinition(AppSettings.WeChatPayManagement.IsActive, GetFromAppSettings(AppSettings.WeChatPayManagement.IsActive, "false"),scopes: SettingScopes.Tenant|SettingScopes.Application),

                new SettingDefinition(AppSettings.AliPayManagement.AppId, GetFromAppSettings(AppSettings.AliPayManagement.AppId, ""),scopes: SettingScopes.Tenant|SettingScopes.Application),
                new SettingDefinition(AppSettings.AliPayManagement.Uid, GetFromAppSettings(AppSettings.AliPayManagement.Uid, ""),scopes: SettingScopes.Tenant|SettingScopes.Application),
                new SettingDefinition(AppSettings.AliPayManagement.Gatewayurl, GetFromAppSettings(AppSettings.AliPayManagement.Gatewayurl, "https://openapi.alipay.com/gateway.do"),scopes: SettingScopes.Tenant|SettingScopes.Application),
                new SettingDefinition(AppSettings.AliPayManagement.AlipayPublicKey, GetFromAppSettings(AppSettings.AliPayManagement.AlipayPublicKey, ""),scopes: SettingScopes.Tenant|SettingScopes.Application),
                new SettingDefinition(AppSettings.AliPayManagement.AlipaySignPublicKey, GetFromAppSettings(AppSettings.AliPayManagement.AlipaySignPublicKey, ""),scopes: SettingScopes.Tenant|SettingScopes.Application),
                new SettingDefinition(AppSettings.AliPayManagement.PrivateKey, GetFromAppSettings(AppSettings.AliPayManagement.PrivateKey, ""),scopes: SettingScopes.Tenant|SettingScopes.Application),
                new SettingDefinition(AppSettings.AliPayManagement.CharSet, GetFromAppSettings(AppSettings.AliPayManagement.CharSet, "utf-8"),scopes: SettingScopes.Tenant|SettingScopes.Application),
                new SettingDefinition(AppSettings.AliPayManagement.Notify, GetFromAppSettings(AppSettings.AliPayManagement.Notify, ""),scopes: SettingScopes.Tenant|SettingScopes.Application),
                new SettingDefinition(AppSettings.AliPayManagement.SignType, GetFromAppSettings(AppSettings.AliPayManagement.SignType, "RSA2"),scopes: SettingScopes.Tenant|SettingScopes.Application),
                new SettingDefinition(AppSettings.AliPayManagement.IsKeyFromFile, GetFromAppSettings(AppSettings.AliPayManagement.IsKeyFromFile, "false"),scopes: SettingScopes.Tenant|SettingScopes.Application),
                new SettingDefinition(AppSettings.AliPayManagement.IsActive, GetFromAppSettings(AppSettings.AliPayManagement.IsActive, "false"),scopes: SettingScopes.Tenant|SettingScopes.Application),

            };
        }

        private IEnumerable<SettingDefinition> GetTenantSettings()
        {
            return new[]
            {
                new SettingDefinition(AppSettings.UserManagement.AllowSelfRegistration, GetFromAppSettings(AppSettings.UserManagement.AllowSelfRegistration, "true"), scopes: SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault, GetFromAppSettings(AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault, "false"), scopes: SettingScopes.Tenant),
                new SettingDefinition(AppSettings.UserManagement.UseCaptchaOnRegistration, GetFromAppSettings(AppSettings.UserManagement.UseCaptchaOnRegistration, "true"), scopes: SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettings.TenantManagement.BillingLegalName, GetFromAppSettings(AppSettings.TenantManagement.BillingLegalName, ""), scopes: SettingScopes.Tenant),
                new SettingDefinition(AppSettings.TenantManagement.BillingAddress, GetFromAppSettings(AppSettings.TenantManagement.BillingAddress, ""), scopes: SettingScopes.Tenant),
                new SettingDefinition(AppSettings.TenantManagement.BillingTaxNumber, GetFromAppSettings(AppSettings.TenantManagement.BillingTaxNumber,""), scopes: SettingScopes.Tenant),
                new SettingDefinition(AppSettings.TenantManagement.BillingContact, GetFromAppSettings(AppSettings.TenantManagement.BillingContact,""), scopes: SettingScopes.Tenant),
                new SettingDefinition(AppSettings.TenantManagement.BillingBankAccount, GetFromAppSettings(AppSettings.TenantManagement.BillingBankAccount,""), scopes: SettingScopes.Tenant),
                new SettingDefinition(AppSettings.TenantManagement.BillingBank, GetFromAppSettings(AppSettings.TenantManagement.BillingBank,""), scopes: SettingScopes.Tenant),


            };
        }

        private IEnumerable<SettingDefinition> GetSharedSettings()
        {
            return new[]
            {
                new SettingDefinition(AppSettings.UserManagement.TwoFactorLogin.IsGoogleAuthenticatorEnabled, GetFromAppSettings(AppSettings.UserManagement.TwoFactorLogin.IsGoogleAuthenticatorEnabled, "false"), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true)
            };
        }

        private string GetFromAppSettings(string name, string defaultValue = null)
        {
            return GetFromSettings("App:" + name, defaultValue);
        }

        private string GetFromSettings(string name, string defaultValue = null)
        {
            return _appConfiguration[name] ?? defaultValue;
        }

        private IEnumerable<SettingDefinition> GetSmsCodeSettings()
        {
            return new[] {
                new SettingDefinition(AppSettings.AliSmsCodeManagement.IsEnabled, GetFromAppSettings(AppSettings.AliSmsCodeManagement.IsEnabled, "false"),scopes: SettingScopes.Tenant|SettingScopes.Application),
                new SettingDefinition(AppSettings.AliSmsCodeManagement.AccessKeyId, GetFromAppSettings(AppSettings.AliSmsCodeManagement.AccessKeySecret, ""),scopes: SettingScopes.Tenant|SettingScopes.Application),
                new SettingDefinition(AppSettings.AliSmsCodeManagement.AccessKeySecret, GetFromAppSettings(AppSettings.AliSmsCodeManagement.AccessKeySecret, ""),scopes: SettingScopes.Tenant|SettingScopes.Application),
                new SettingDefinition(AppSettings.AliSmsCodeManagement.SignName, GetFromAppSettings(AppSettings.AliSmsCodeManagement.SignName, ""),scopes: SettingScopes.Tenant|SettingScopes.Application),
                new SettingDefinition(AppSettings.AliSmsCodeManagement.TemplateCode, GetFromAppSettings(AppSettings.AliSmsCodeManagement.TemplateCode, ""),scopes: SettingScopes.Tenant|SettingScopes.Application),
                new SettingDefinition(AppSettings.AliSmsCodeManagement.TemplateParam, GetFromAppSettings(AppSettings.AliSmsCodeManagement.TemplateParam, ""),scopes: SettingScopes.Tenant|SettingScopes.Application)
            };
        }
    }
}
