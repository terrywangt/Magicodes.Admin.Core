namespace Magicodes.Admin.Configuration
{
    /// <summary>
    /// Defines string constants for setting names in the application.
    /// See <see cref="AppSettingProvider"/> for setting definitions.
    /// </summary>
    public static class AppSettings
    {
        public static class HostManagement
        {
            public const string BillingLegalName = "App.HostManagement.BillingLegalName";
            public const string BillingAddress = "App.HostManagement.BillingAddress";
            public const string BillingTaxNumber = "App.HostManagement.BillingTaxNumber";
            public const string BillingContact = "App.HostManagement.BillingContact";
            public const string BillingBankAccount = "App.HostManagement.BillingBankAccount";
            public const string BillingBank = "App.HostManagement.BillingBank";

        }

        public static class UiManagement
        {
            public const string LayoutType = "App.UiManagement.LayoutType";
            public const string ContentSkin = "App.UiManagement.ContentSkin";
            public const string Theme = "App.UiManagement.Theme";

            public static class Header
            {
                public const string DesktopFixedHeader = "App.UiManagement.Header.DesktopFixedHeader";
                public const string DesktopMinimizeMode = "App.UiManagement.Header.DesktopMinimizeMode";
                public const string MobileFixedHeader = "App.UiManagement.Header.MobileFixedHeader"; 
                public const string Skin = "App.UiManagement.Header.Skin"; 
                public const string DisplaySubmenuArrowDesktop = "App.UiManagement.Header.DisplaySubmenuArrow_Desktop"; 
            }

            public static class LeftAside
            {
                public const string Position = "App.UiManagement.Left.Position";
                public const string AsideSkin = "App.UiManagement.Left.AsideSkin";
                public const string FixedAside = "App.UiManagement.Left.FixedAside"; 
                public const string AllowAsideMinimizing = "App.UiManagement.Left.AllowAsideMinimizing"; 
                public const string DefaultMinimizedAside = "App.UiManagement.Left.DefaultMinimizedAside"; 
                public const string AllowAsideHiding = "App.UiManagement.Left.AllowAsideHiding"; 
                public const string DefaultHiddenAside = "App.UiManagement.Left.DefaultHiddenAside"; 
                public const string SubmenuToggle = "App.UiManagement.Left.SubmenuToggle"; 
                public const string DropdownSubmenuSkin = "App.UiManagement.Left.DropdownSubmenuSkin"; 
                public const string DropdownSubmenuArrow = "App.UiManagement.Left.DropdownSubmenuArrow"; 
            }

            public static class Footer
            {
                public const string FixedFooter = "App.UiManagement.Footer.FixedFooter";
            }
        }


        public static class TenantManagement
        {
            public const string AllowSelfRegistration = "App.TenantManagement.AllowSelfRegistration";
            public const string IsNewRegisteredTenantActiveByDefault = "App.TenantManagement.IsNewRegisteredTenantActiveByDefault";
            public const string UseCaptchaOnRegistration = "App.TenantManagement.UseCaptchaOnRegistration";
            public const string DefaultEdition = "App.TenantManagement.DefaultEdition";
            public const string SubscriptionExpireNotifyDayCount = "App.TenantManagement.SubscriptionExpireNotifyDayCount";
            public const string BillingLegalName = "App.HostManagement.BillingLegalName";
            public const string BillingAddress = "App.HostManagement.BillingAddress";
            public const string BillingTaxNumber = "App.HostManagement.BillingTaxNumber";
            public const string BillingContact = "App.HostManagement.BillingContact";
            public const string BillingBankAccount = "App.HostManagement.BillingBankAccount";
            public const string BillingBank = "App.HostManagement.BillingBank";
        }

        public static class UserManagement
        {
            public static class TwoFactorLogin
            {
                public const string IsGoogleAuthenticatorEnabled = "App.UserManagement.TwoFactorLogin.IsGoogleAuthenticatorEnabled";
            }

            public const string AllowSelfRegistration = "App.UserManagement.AllowSelfRegistration";
            public const string IsNewRegisteredUserActiveByDefault = "App.UserManagement.IsNewRegisteredUserActiveByDefault";
            public const string UseCaptchaOnRegistration = "App.UserManagement.UseCaptchaOnRegistration";
            public const string SmsVerificationEnabled = "App.UserManagement.SmsVerificationEnabled";

        }
        
        public static class Recaptcha
        {
            public const string SiteKey = "Recaptcha.SiteKey";
        }

        public static class CacheKeys
        {
            public const string TenantRegistrationCache = "TenantRegistrationCache";
        }

        public static class WeChatMiniProgram
        {
            public const string IsActive = "App.WeChatMiniProgram.IsActive";
            public const string AppId = "App.WeChatMiniProgram.AppId";
            public const string AppSecret = "App.WeChatMiniProgram.AppSecret";
        }

        public static class WeChatPayManagement
        {
            public const string AppId = "App.WeChatPayManagement.AppId";
            public const string MchId = "App.WeChatPayManagement.MchId";
            public const string TenPayKey = "App.WeChatPayManagement.TenPayKey";
            public const string PayNotifyUrl = "App.WeChatPayManagement.PayNotifyUrl";
            public const string IsActive = "App.WeChatPayManagement.IsActive";
        }

        /// <summary>
        /// 腾讯云存储配置
        /// </summary>
        public static class TencentStorageManagement
        {
            public const string IsEnabled = "App.TencentStorageManagement.IsEnabled";

            public const string AppId = "App.TencentStorageManagement.AppId";

            public const string SecretId = "App.TencentStorageManagement.SecretId";

            public const string SecretKey = "App.TencentStorageManagement.SecretKey";

            public const string Region = "App.TencentStorageManagement.Region";

            public const string BucketName = "App.TencentStorageManagement.BucketName";
        }

        /// <summary>
        /// 支付宝配置
        /// </summary>
        public static class AliPayManagement
        {
            public const string AppId = "App.AliPayManagement.AppId";
            public const string Uid = "App.AliPayManagement.Uid";
            public const string Gatewayurl = "App.AliPayManagement.Gatewayurl";
            public const string AlipayPublicKey = "App.AliPayManagement.AlipayPublicKey";
            public const string AlipaySignPublicKey = "App.AliPayManagement.AlipaySignPublicKey";
            public const string PrivateKey = "App.AliPayManagement.PrivateKey";
            public const string CharSet = "App.AliPayManagement.CharSet";
            public const string Notify = "App.AliPayManagement.Notify";
            public const string SignType = "App.AliPayManagement.SignType";
            public const string IsKeyFromFile = "App.AliPayManagement.IsKeyFromFile";
            public const string IsActive = "App.AliPayManagement.IsActive";
        }

        /// <summary>
        /// 国际支付宝配置
        /// </summary>
        public static class GlobalAliPayManagement
        {
            /// <summary>
            /// MD5密钥，安全检验码，由数字和字母组成的32位字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm
            /// </summary>
            public const string Key = "App.GlobalAliPayManagement.Key";
            /// <summary>
            /// 合作商户uid(合作身份者ID，签约账号，以2088开头由16位纯数字组成的字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm)
            /// </summary>
            public const string Partner = "App.GlobalAliPayManagement.Partner";
            /// <summary>
            /// 支付宝网关
            /// </summary>
            public const string Gatewayurl = "App.GlobalAliPayManagement.Gatewayurl";

            /// <summary>
            /// 服务器异步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数,必须外网可以正常访问
            /// </summary>
            public const string Notify = "App.GlobalAliPayManagement.Notify";

            /// <summary>
            /// 页面跳转同步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数，必须外网可以正常访问
            /// </summary>
            public const string ReturnUrl = "App.GlobalAliPayManagement.ReturnUrl";

            /// <summary>
            /// 结算币种
            /// </summary>
            public const string Currency = "App.GlobalAliPayManagement.Currency";

            public const string IsActive = "App.GlobalAliPayManagement.IsActive";

            /// <summary>
            /// 分账信息
            /// </summary>
            public const string SplitFundSettings = "App.GlobalAliPayManagement.SplitFundSettings";
        }

        public static class AliSmsCodeManagement
        {
            public const string IsEnabled = "App.AliSmsCodeManagement.IsEnabled";

            public const string AccessKeyId = "App.AliSmsCodeManagement.AccessKeyId";

            public const string AccessKeySecret = "App.AliSmsCodeManagement.AccessKeySecret";

            public const string SignName = "App.AliSmsCodeManagement.SignName";

            public const string TemplateCode = "App.AliSmsCodeManagement.TemplateCode";

            public const string TemplateParam = "App.AliSmsCodeManagement.TemplateParam";
        }

        public static class AliStorageManagement
        {
            public const string IsEnabled = "App.AliStorageManagement.IsEnabled";

            public const string AccessKeyId = "App.AliStorageManagement.AccessKeyId";

            public const string AccessKeySecret = "App.AliStorageManagement.AccessKeySecret";

            public const string EndPoint = "App.AliStorageManagement.EndPoint";
        }
    }
}