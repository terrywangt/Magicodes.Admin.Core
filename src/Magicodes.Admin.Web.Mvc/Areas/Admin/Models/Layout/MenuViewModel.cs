using Abp.Application.Navigation;

namespace Magicodes.Admin.Web.Areas.Admin.Models.Layout
{
    public class MenuViewModel
    {
        public UserMenu Menu { get; set; }

        public string CurrentPageName { get; set; }
    }
}