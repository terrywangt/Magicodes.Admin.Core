using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.Web.Models;

namespace Magicodes.Admin.App.Controllers
{
    /// <summary>
    /// APP API控制器基类
    /// </summary>
    [WrapResult]
    public abstract class AppControllerBase: AbpController
    {
        /// <summary>
        /// API前缀路径
        /// </summary>
        protected const string ApiPrefix = "api/";

        protected AppControllerBase()
        {
            //LocalizationSourceName = AdminConsts.LocalizationSourceName;
        }
    }
}
