using Abp.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace Magicodes.Admin.Web.Controllers
{
    public class HomeController : AdminControllerBase
    {
        public IActionResult Index()
        {
            return Redirect("/swagger/ui");
        }
    }
}
