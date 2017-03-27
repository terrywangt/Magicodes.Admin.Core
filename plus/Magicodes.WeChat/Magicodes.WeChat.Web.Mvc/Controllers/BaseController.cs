using Abp.AspNetCore.Mvc.Controllers;
using Magicodes.Admin.Web.Controllers;
using Magicodes.WeChat.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magicodes.WeChat.Web.Mvc.Controllers
{
    [Area(WeChatConsts.AdminAreaName)]
    public class BaseController : AdminPluginControllerBase
    {
        const string PlusName = "Magicodes.WeChat.Web.Mvc";
        public BaseController() : base(WeChatConsts.LocalizationSourceName)
        {

        }

        public override ViewResult View()
        {
            return PluginView(PlusName) as ViewResult;
        }

        public override ViewResult View(object model)
        {
            return PluginView(PlusName, model) as ViewResult; ;
        }
    }
}
