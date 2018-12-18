using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.Domain.Repositories;
using Cms.Host.Models;
using Magicodes.Admin.Contents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

namespace Cms.Host.Controllers
{
    public class HomeController : AbpController
    {



        public IActionResult Index()
        {
            return Content("Test");
        }


    }
}