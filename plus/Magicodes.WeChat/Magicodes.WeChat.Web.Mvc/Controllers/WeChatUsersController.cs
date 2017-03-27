using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Magicodes.WeChat.Web.Mvc.Controllers
{
    public class WeChatUsersController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
