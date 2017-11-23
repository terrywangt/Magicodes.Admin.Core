using Microsoft.AspNetCore.Mvc;
using Magicodes.Admin.Web.Controllers;

namespace Magicodes.Admin.Web.Public.Controllers
{
    public class HomeController : AdminControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}