using System.Diagnostics;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Cms.Host.Models;
using Magicodes.Admin.Contents;
using Magicodes.Admin.Contents.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Host.Controllers
{
    public class HomeController : AbpController
    {
        private readonly IArticleInfoAppService _articleInfoAppService;
        private readonly IColumnInfoAppService _columnInfoAppService;

        public HomeController(IColumnInfoAppService columnInfoAppService, IArticleInfoAppService articleInfoAppService)
        {
            _columnInfoAppService = columnInfoAppService;
            _articleInfoAppService = articleInfoAppService;
        }


        public async Task<IActionResult> Index()
        {
            var getColumnInfosInput = new GetColumnInfosInput
            {
                SkipCount = 0,
                MaxResultCount = 1000
            };
            var columnInfos = await _columnInfoAppService.GetColumnInfos(getColumnInfosInput);
            return View(columnInfos);
        }

        public IActionResult Article(long cid)
        {
            return View();
        }

        public IActionResult Detail(string url)
        {
            return NotFound();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}