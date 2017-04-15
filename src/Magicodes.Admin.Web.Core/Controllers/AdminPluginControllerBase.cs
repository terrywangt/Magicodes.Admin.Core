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

        /// <summary>
        /// 插件视图
        /// </summary>
        /// <param name="plusName">插件短名</param>
        /// <param name="view">视图名称</param>
        /// <param name="model">模型</param>
        /// <returns></returns>
        public virtual ViewResult PluginView(string plusName, string view = null, object model = null)
        {
            var viewPath = view;
            if (string.IsNullOrWhiteSpace(viewPath))
            {
                viewPath = string.Format("{0}/{1}.cshtml", RouteData.Values["controller"], RouteData.Values["action"]);
            }
            else if (viewPath.IndexOf("/") == -1)
            {
                viewPath = string.Format("{0}/{1}{2}", RouteData.Values["controller"], view, view.EndsWith(".cshtml") ? string.Empty : ".cshtml");
            }
            if (!viewPath.StartsWith("~/wwwroot"))
            {
                viewPath = "~/wwwroot/PlugIns/" + plusName + "/Views/" + viewPath.TrimStart('~').TrimStart('/');
            }
            return model == null ? View(viewPath) : View(viewPath, model);
        }

        /// <summary>
        /// 插件部分视图
        /// </summary>
        /// <param name="plusName">插件短名</param>
        /// <param name="view">视图名称</param>
        /// <param name="model">模型</param>
        /// <returns></returns>
        public virtual PartialViewResult PluginPartialView(string plusName, string view = null, object model = null)
        {
            var viewPath = view;
            if (string.IsNullOrWhiteSpace(viewPath))
            {
                viewPath = string.Format("{0}/{1}.cshtml", RouteData.Values["controller"], RouteData.Values["action"]);
            }
            else if (viewPath.IndexOf("/") == -1)
            {
                viewPath = string.Format("{0}/{1}{2}", RouteData.Values["controller"], view, view.EndsWith(".cshtml") ? string.Empty : ".cshtml");
            }

            if (!viewPath.StartsWith("~/wwwroot"))
            {
                viewPath = "~/wwwroot/PlugIns/" + plusName + "/Views/" + viewPath.TrimStart('~').TrimStart('/');
            }
            return model == null ? PartialView(viewPath) : PartialView(viewPath, model);
        }
    }
}
