using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magicodes.Home.Controllers
{
    public class HomeController: PluginControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult Customer()
        {
            return View();
        }
        public IActionResult Product()
        {
            return View();
        }
        public IActionResult ProductPrice()
        {
            return View();
        }
        public IActionResult DataWeiChat()
        {
            return View();
        }
        public IActionResult DataShop()
        {
            return View();
        }
    }
}
