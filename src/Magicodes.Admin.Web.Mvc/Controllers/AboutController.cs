using Microsoft.AspNetCore.Mvc;

namespace Magicodes.Admin.Web.Controllers
{
    public class AboutController : AdminControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}