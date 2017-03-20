using Abp.AspNetCore.Mvc.Controllers;
using Magicodes.Admin.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magicodes.Home.Controllers
{
    [Area("Web")]
    public class PluginControllerBase: AdminPluginControllerBase
    {
        public PluginControllerBase() : base(HomeConsts.LocalizationSourceName)
        {
            ViewData["ContentRootUrl"] = "/PlugIns/Magicodes.Home/wwwroot";
            //ViewBag.ContentRootUrl = "/PlugIns/Magicodes.Home/wwwroot";
        }

        public override ViewResult View()
        {
            var view = string.Format("{0}/{1}.cshtml", RouteData.Values["controller"], RouteData.Values["action"]);
            return PluginView(view, "Magicodes.Home") as ViewResult;
        }
    }
}
