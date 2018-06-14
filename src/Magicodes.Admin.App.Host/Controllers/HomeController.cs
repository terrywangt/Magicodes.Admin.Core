using Abp.AspNetCore.Mvc.Controllers;
using Abp.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace Magicodes.Admin.Web.Controllers
{
    public class HomeController : AbpController
    {
        [DisableAuditing]
        public IActionResult Index()
        {
            //跳转到接口文档
            return Redirect("/swagger");
        }
    }
}
