using Microsoft.AspNetCore.Mvc;

namespace Magicodes.Admin.Web.Controllers
{
    public class HomeController : AdminControllerBase
    {
        public IActionResult Index(string redirect = "")
        {
            if (redirect == "TenantRegistration")
            {
                return RedirectToAction("SelectEdition", "TenantRegistration");
            }

            return AbpSession.UserId.HasValue ? 
                RedirectToAction("Index", "Home", new { area = "Admin" }) : 
                RedirectToAction("Login", "Account");
        }
    }
}