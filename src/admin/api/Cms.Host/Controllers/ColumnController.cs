using Magicodes.Admin.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.Runtime.Caching;
using Magicodes.Admin.Contents.Dto;
using Magicodes.Admin.Dto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Cms.Host.Controllers
{

    public class ColumnController: AbpController
    {
        private readonly IColumnInfoAppService _columnInfoAppService;
        private readonly ICacheManager _cacheManager;


        public ColumnController(IColumnInfoAppService columnInfoAppService, ICacheManager cacheManager)
        {
            _columnInfoAppService = columnInfoAppService;
            _cacheManager = cacheManager;
        }

        public async Task<IActionResult> Index()
        {
            var getColumnInfosInput = new GetColumnInfosInput
            {
                SkipCount = 0,
                MaxResultCount = 1000
            };
            var columnInfos = await _columnInfoAppService.GetColumnInfos(getColumnInfosInput);
            var headerNavs = await _columnInfoAppService.GetChildrenColumnInfos(new GetChildrenColumnInfosInput
            {
                IsHeaderNav = true,
                IsOnlyGetRecycleData = false
            });
            //ViewData["Nav"] = navs; 
            return View(columnInfos);
        }
    }
}
