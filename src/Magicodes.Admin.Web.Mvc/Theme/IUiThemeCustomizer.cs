using Abp.Configuration;
using Abp.Dependency;
using Abp.Threading;
using Magicodes.Admin.Configuration;

namespace Magicodes.Admin.Web.Theme
{
    public interface IUiThemeCustomizer : ITransientDependency
    {
        string LayoutType { get; }
        string PageLoader { get; }
        string ContentSkin { get; }

        bool HeaderDesktopFixedHeader { get; }
        string HeaderDesktopMinimizeMode { get; }
        bool HeaderMobileFixedHeader { get; }
        string HeaderDropdownSkinDesktop { get; }
        bool HeaderDisplaySubmenuArrowDesktop { get; }
        string HeaderDropdownSkin { get; }

        string LeftAsidePosition { get; }
        string LeftAsideAsideSkin { get; }
        bool LeftAsideFixedAside { get; }
        bool LeftAsideAllowAsideMinimizing { get; }
        bool LeftAsideDefaultMinimizedAside { get; }
        bool LeftAsideAllowAsideHiding { get; }
        bool LeftAsideDefaultHiddenAside { get; }
        string LeftAsideSubmenuToggle { get; }
        string LeftAsideDropdownSubmenuSkin { get; }
        bool LeftAsideDropdownSubmenuArrow { get; }

        bool IsLeftMenuUsed { get; }
        bool IsTopMenuUsed { get; }

        bool FooterFixedFooter { get; }
    }

    public class MetronicUiThemeCustomizer : IUiThemeCustomizer
    {
        private readonly SettingManager _settingManager;

        public MetronicUiThemeCustomizer(SettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        private string GetSettingValue(string settingName)
        {
            return AsyncHelper.RunSync(() => _settingManager.GetSettingValueAsync(settingName));
        }

        private bool GetSettingValueAsBoolean(string settingName)
        {
            return AsyncHelper.RunSync(() => _settingManager.GetSettingValueAsync<bool>(settingName));
        }

        public string LayoutType => GetSettingValue(AppSettings.UiManagement.LayoutType);
        public string PageLoader => GetSettingValue(AppSettings.UiManagement.PageLoader);
        public string ContentSkin => GetSettingValue(AppSettings.UiManagement.ContentSkin);

        public bool HeaderDesktopFixedHeader => GetSettingValueAsBoolean(AppSettings.UiManagement.Header.DesktopFixedHeader);
        public string HeaderDesktopMinimizeMode => GetSettingValue(AppSettings.UiManagement.Header.DesktopMinimizeMode);
        public bool HeaderMobileFixedHeader => GetSettingValueAsBoolean(AppSettings.UiManagement.Header.MobileFixedHeader);
        public string HeaderDropdownSkinDesktop => GetSettingValue(AppSettings.UiManagement.Header.DropdownSkinDesktop);
        public bool HeaderDisplaySubmenuArrowDesktop => GetSettingValueAsBoolean(AppSettings.UiManagement.Header.DisplaySubmenuArrowDesktop);
        public string HeaderDropdownSkin => GetSettingValue(AppSettings.UiManagement.Header.DropdownSkin);

        public string LeftAsidePosition => GetSettingValue(AppSettings.UiManagement.LeftAside.Position);
        public string LeftAsideAsideSkin => GetSettingValue(AppSettings.UiManagement.LeftAside.AsideSkin);
        public bool LeftAsideFixedAside => GetSettingValueAsBoolean(AppSettings.UiManagement.LeftAside.FixedAside);
        public bool LeftAsideAllowAsideMinimizing => GetSettingValueAsBoolean(AppSettings.UiManagement.LeftAside.AllowAsideMinimizing);
        public bool LeftAsideDefaultMinimizedAside => GetSettingValueAsBoolean(AppSettings.UiManagement.LeftAside.DefaultMinimizedAside);
        public bool LeftAsideAllowAsideHiding => GetSettingValueAsBoolean(AppSettings.UiManagement.LeftAside.AllowAsideHiding);
        public bool LeftAsideDefaultHiddenAside => GetSettingValueAsBoolean(AppSettings.UiManagement.LeftAside.DefaultHiddenAside);
        public string LeftAsideSubmenuToggle => GetSettingValue(AppSettings.UiManagement.LeftAside.SubmenuToggle);
        public string LeftAsideDropdownSubmenuSkin => GetSettingValue(AppSettings.UiManagement.LeftAside.DropdownSubmenuSkin);
        public bool LeftAsideDropdownSubmenuArrow => GetSettingValueAsBoolean(AppSettings.UiManagement.LeftAside.DropdownSubmenuArrow);

        public bool IsLeftMenuUsed => LeftAsidePosition == "left";
        public bool IsTopMenuUsed => LeftAsidePosition == "top";

        public bool FooterFixedFooter => GetSettingValueAsBoolean(AppSettings.UiManagement.Footer.FixedFooter);
    }
}
