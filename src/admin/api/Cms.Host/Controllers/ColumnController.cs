using Magicodes.Admin.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Magicodes.Admin.Contents.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Host.Controllers
{

    public class ColumnController: AbpController
    {
        private readonly IColumnInfoAppService _columnInfoAppService;

        public ColumnController(IColumnInfoAppService columnInfoAppService)
        {
            _columnInfoAppService = columnInfoAppService;
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
    }
}
