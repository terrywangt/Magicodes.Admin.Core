using Abp.Application.Navigation;

namespace Magicodes.Admin.Web.Public.Views.Shared.Components.Header
{
    public class HeaderMenuItemViewModel
    {
        public UserMenuItem MenuItem { get; set; }

        public int CurrentLevel { get; set; }

        public string CurrentPageName { get; set; }

        public HeaderMenuItemViewModel(UserMenuItem menuItem, int currentLevel, string currentPageName)
        {
            MenuItem = menuItem;
            CurrentLevel = currentLevel;
            CurrentPageName = currentPageName;
        }
    }
}
