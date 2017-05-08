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
            PlusName = "Magicodes.Home";
            ViewData["ContentRootUrl"] = "/PlugIns/Magicodes.Home/wwwroot";
            //ViewBag.ContentRootUrl = "/PlugIns/Magicodes.Home/wwwroot";
        }
    }
}
