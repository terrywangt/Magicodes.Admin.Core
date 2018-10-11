namespace Magicodes.Admin.Configuration.Dto
{
    public class UiCustomizationSettingsEditDto
    {
        public UiCustomizationLayoutSettingsEditDto Layout { get; set; }

        public UiCustomizationHeaderSettingsEditDto Header { get; set; }

        public UiCustomizationMenuSettingsEditDto Menu { get; set; }

        public UiCustomizationFooterSettingsEditDto Footer { get; set; }
    }

    public class UiCustomizationLayoutSettingsEditDto
    {
        public string LayoutType { get; set; }

        public string ContentSkin { get; set; }

        public string Theme { get; set; }
    }

    public class UiCustomizationHeaderSettingsEditDto
    {
        public bool DesktopFixedHeader { get; set; }

        public string DesktopMinimizeMode { get; set; }

        public bool MobileFixedHeader { get; set; }

        public string HeaderSkin { get; set; }

        public bool DisplaySubmenuArrowDesktop { get; set; }
    }

    public class UiCustomizationMenuSettingsEditDto
    {
        public string Position { get; set; }

        public string AsideSkin { get; set; }

        public bool FixedAside { get; set; }

        public bool AllowAsideMinimizing { get; set; }

        public bool DefaultMinimizedAside { get; set; }

        public bool AllowAsideHiding { get; set; }

        public bool DefaultHiddenAside { get; set; }

        public string DropdownSubmenuSkin { get; set; }

        public bool DropdownSubmenuArrow { get; set; }
    }

    public class UiCustomizationFooterSettingsEditDto
    {
        public bool FixedFooter { get; set; }
    }
}