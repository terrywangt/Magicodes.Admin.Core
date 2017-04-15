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

        #region 插件视图
        public virtual IActionResult PluginView(string view, string plusName)
        {
            if (view.StartsWith("~/wwwroot"))
            {
                return View(view);
            }
            else
                return View("~/wwwroot/PlugIns/" + plusName + "/Views/" + view.TrimStart('~').TrimStart('/'));
        }

        public virtual IActionResult PluginView(string view, string plusName, object model)
        {
            if (view.StartsWith("~/wwwroot"))
            {
                return View(view, model);
            }
            else
                return View("~/wwwroot/PlugIns/" + plusName + "/Views/" + view.TrimStart('~').TrimStart('/'), model);
        }

        public virtual IActionResult PluginView(string plusName)
        {
            var view = string.Format("{0}/{1}.cshtml", RouteData.Values["controller"], RouteData.Values["action"]);
            return PluginView(view, plusName);
        }

        public virtual IActionResult PluginView(string plusName, object model)
        {
            var view = string.Format("{0}/{1}.cshtml", RouteData.Values["controller"], RouteData.Values["action"]);
            return PluginView(view, plusName, model);
        }
        #endregion

        #region 插件部分视图
        public virtual PartialViewResult PluginPartialView(string view, string plusName)
        {
            if (view.StartsWith("~/wwwroot"))
            {
                return PartialView(view);
            }
            else
                return PartialView("~/wwwroot/PlugIns/" + plusName + "/Views/" + view.TrimStart('~').TrimStart('/'));
        }

        public virtual PartialViewResult PluginPartialView(string view, string plusName, object model)
        {
            if (view.StartsWith("~/wwwroot"))
            {
                return PartialView(view, model);
            }
            else
                return PartialView("~/wwwroot/PlugIns/" + plusName + "/Views/" + view.TrimStart('~').TrimStart('/'), model);
        }

        public virtual PartialViewResult PluginPartialView(string plusName)
        {
            var view = string.Format("{0}/{1}.cshtml", RouteData.Values["controller"], RouteData.Values["action"]);
            return PluginPartialView(view, plusName);
        }

        public virtual PartialViewResult PluginPartialView(string plusName, object model)
        {
            var view = string.Format("{0}/{1}.cshtml", RouteData.Values["controller"], RouteData.Values["action"]);
            return PluginPartialView(view, plusName, model);
        }
        #endregion
    }
}
