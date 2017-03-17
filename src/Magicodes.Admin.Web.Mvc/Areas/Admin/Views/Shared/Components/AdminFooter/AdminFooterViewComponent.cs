using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Magicodes.Admin.Web.Areas.Admin.Models.Layout;
using Magicodes.Admin.Web.Session;

namespace Magicodes.Admin.Web.Areas.Admin.Views.Shared.Components.AdminFooter
{
    public class AdminFooterViewComponent : ViewComponent
    {
        private readonly IPerRequestSessionCache _sessionCache;

        public AdminFooterViewComponent(IPerRequestSessionCache sessionCache)
        {
            _sessionCache = sessionCache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var footerModel = new FooterViewModel
            {
                LoginInformations = await _sessionCache.GetCurrentLoginInformationsAsync()
            };

            return View(footerModel);
        }
    }
}
