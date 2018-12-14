using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Cms.Host.Models;
using Magicodes.Admin.Contents;

namespace Cms.Host.Controllers
{
    public class HomeController : AbpController
    {

        private readonly IRepository<ColumnInfo, long> _columnInfoRepository;

        public HomeController(IRepository<ColumnInfo, long> columnInfoRepository)
        {
            _columnInfoRepository = columnInfoRepository;
        }

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
