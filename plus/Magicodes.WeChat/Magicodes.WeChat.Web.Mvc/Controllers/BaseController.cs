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
        public BaseController() : base(WeChatConsts.LocalizationSourceName)
        {
            PlusName = "Magicodes.WeChat.Web.Mvc";
        }
    }
}
