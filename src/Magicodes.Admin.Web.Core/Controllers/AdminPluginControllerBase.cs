using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magicodes.Admin.Web.Controllers
{
    public class AdminPluginControllerBase : AbpController
    {
        protected AdminPluginControllerBase(string localizationSourceName)
        {
            LocalizationSourceName = localizationSourceName;
        }

        public virtual IActionResult PluginView(string view, string plusName)
        {
            if (view.StartsWith("~/wwwroot"))
            {
                return View(view);
            }
            else
                return View("~/wwwroot/PlugIns/" + plusName + "/Views/" + view.TrimStart('~').TrimStart('/'));
        }
    }
}
