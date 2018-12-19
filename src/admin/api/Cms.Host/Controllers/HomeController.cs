using System.Diagnostics;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Cms.Host.Models;
using Magicodes.Admin.Contents;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Host.Controllers
{
    public class HomeController : AbpController
    {
        private readonly IArticleInfoAppService _articleInfoAppService;

        public HomeController(IArticleInfoAppService articleInfoAppService)
        {
            _articleInfoAppService = articleInfoAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> StaticRouter(string url)
        {
            var articleInfo = await _articleInfoAppService.GetArticleInfoByStaticUrl(url);
            if (articleInfo!=null)
            {
                return RedirectToAction("Detail", "Article", new
                {
                    articleInfo.ArticleInfo.Id,
                });
            }
            return NotFound();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}