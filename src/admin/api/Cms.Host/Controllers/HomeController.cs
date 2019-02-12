using System.Diagnostics;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Cms.Host.Common;
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


        public HomeController(IArticleInfoAppService articleInfoAppService, IColumnInfoAppService columnInfoAppService)
        {
            _articleInfoAppService = articleInfoAppService;
            _columnInfoAppService = columnInfoAppService;
        }

        public async Task<IActionResult> Index()
        {
            await GetNavs();
            ViewBag.Source = true;
            return View();
        }



        public async Task<IActionResult> StaticRouter(string url)
        {
            var articleInfo = await _articleInfoAppService.GetArticleInfoByStaticUrl(url);
            if (articleInfo != null)
                return RedirectToAction("Detail", "Article", new
                {
                    articleInfo.ArticleInfo.Id
                });
            return NotFound();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        private async Task GetNavs()
        {
            var headerNav = await _columnInfoAppService.GetChildrenColumnInfos(new GetChildrenColumnInfosInput
            {
                IsHeaderNav = true,
                IsOnlyGetRecycleData = false
            });
            var footerNav = await _columnInfoAppService.GetChildrenColumnInfos(new GetChildrenColumnInfosInput
            {
                IsFooterNav = true,
                IsOnlyGetRecycleData = false
            });
            NavHelper.HeaderNav = headerNav.Data;
            NavHelper.FooterNav = footerNav.Data;
            ViewData["HeaderNav"] = NavHelper.HeaderNav;
            ViewData["FooterNav"] = NavHelper.FooterNav;
        }
    }
}