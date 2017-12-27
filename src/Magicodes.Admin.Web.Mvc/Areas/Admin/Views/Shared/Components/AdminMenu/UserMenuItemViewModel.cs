using Abp.Application.Navigation;

namespace Magicodes.Admin.Web.Areas.Admin.Views.Shared.Components.AdminSideBar
{
    public class UserMenuItemViewModel
    {
        public UserMenuItem MenuItem { get; set; }

        public string CurrentPageName { get; set; }

        public int MenuItemIndex { get; set; }

        public bool RootLevel { get; set; }
    }
}
