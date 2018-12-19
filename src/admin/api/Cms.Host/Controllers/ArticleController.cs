using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.Runtime.Caching;
using Magicodes.Admin.Contents;
using Magicodes.Admin.Contents.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Host.Controllers
{
    public class ArticleController : AbpController
    {
        private readonly IArticleInfoAppService _articleInfoAppService;
        private readonly ICacheManager _cacheManager;

        public ArticleController(IArticleInfoAppService articleInfoAppService, ICacheManager cacheManager)
        {
            _articleInfoAppService = articleInfoAppService;
            _cacheManager = cacheManager;
        }

        public async Task<IActionResult> Index(long? cid)
        {
            var getArticleInfosInput = new GetArticleInfosInput
            {
                SkipCount = 0,
                MaxResultCount = 1000,
                ColumnId = cid
            };
            var articleInfos = await _articleInfoAppService.GetArticleInfos(getArticleInfosInput);
            return View(articleInfos);
        }

        public async Task<IActionResult> Detail(long id)
        {
            var input = new NullableIdDto<long>
            {
                Id = id
            };
            GetArticleInfoForEditOutput articleInfo;
            var articleInfoCache = await _cacheManager.GetCache<long, GetArticleInfoForEditOutput>("ArticleInfo").GetOrDefaultAsync(id);
            if (articleInfoCache == null)
            {
                articleInfo = await _articleInfoAppService.GetArticleInfoForEdit(input);
                await _cacheManager.GetCache<long, GetArticleInfoForEditOutput>("ArticleInfo")
                    .SetAsync(id, articleInfo);
            }
            else
            {
                articleInfo = articleInfoCache;
            }
            return View(articleInfo);
        }
    }
}