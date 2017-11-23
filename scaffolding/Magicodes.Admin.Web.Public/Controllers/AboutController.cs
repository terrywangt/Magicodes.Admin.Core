using Microsoft.AspNetCore.Mvc;
using Magicodes.Admin.Web.Controllers;

namespace Magicodes.Admin.Web.Public.Controllers
{
    public class AboutController : AdminControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}