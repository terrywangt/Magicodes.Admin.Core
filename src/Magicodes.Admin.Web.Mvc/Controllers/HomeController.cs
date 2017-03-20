using Magicodes.Admin.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Magicodes.Admin.Web.Controllers
{
    public class HomeController : AdminControllerBase
    {
        public ActionResult Index()
        {
            //SettingManager.ChangeSettingForApplicationAsync(AppSettings.TenantManagement.DefaultUrl, "/web/");
            var defaultUrl = SettingManager.GetSettingValueAsync(AppSettings.TenantManagement.DefaultUrl).Result;
            if (!string.IsNullOrEmpty(defaultUrl))
            {
                return Redirect(defaultUrl);
            }
            return View();
        }
    }
}