using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cms.Host.Models;
using Magicodes.Admin.Contents;

namespace Cms.Host.Controllers
{
    public class HomeController : Controller
    {

       //// private IColumnInfoAppService _columnInfoAppService;

       // public HomeController(IColumnInfoAppService columnInfoAppService)
       // {
       //     _columnInfoAppService = columnInfoAppService;
       // }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
