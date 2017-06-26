using System.Threading.Tasks;
using Abp.Application.Navigation;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Magicodes.Admin.Web.Areas.Admin.Models.Layout;
using Magicodes.Admin.Web.Areas.Admin.Startup;
using Magicodes.Admin.Web.Views;

namespace Magicodes.Admin.Web.Areas.Admin.Views.Shared.Components.AdminSideBar
{
    public class AdminSideBarViewComponent : AdminViewComponent
    {
        private readonly IUserNavigationManager _userNavigationManager;
        private readonly IAbpSession _abpSession;

        public AdminSideBarViewComponent(
            IUserNavigationManager userNavigationManager, IAbpSession abpSession)
        {
            _userNavigationManager = userNavigationManager;
            _abpSession = abpSession;
        }

        public async Task<IViewComponentResult> InvokeAsync(string currentPageName = null)
        {
            var sidebarModel = new SidebarViewModel
            {
                Menu = await _userNavigationManager.GetMenuAsync(AdminNavigationProvider.MenuName, _abpSession.ToUserIdentifier()),
                CurrentPageName = currentPageName
            };

            return View(sidebarModel);
        }
    }
}
