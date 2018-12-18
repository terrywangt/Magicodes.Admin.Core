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
    public class ColController : AbpController
    {
        private readonly IRepository<ColumnInfo, long> _columnInfoRepository;
        private readonly List<CmsInfoDto> _columnInfoDtos;
        private readonly List<CmsInfoDto> _articleInfoDtos;

        public ColController(IRepository<ColumnInfo, long> columnInfoRepository)
        {
            _columnInfoRepository = columnInfoRepository;
        }



        //public HomeController()
        //{
        //    _columnInfoDtos = new List<CmsInfoDto>()
        //    {
        //        new CmsInfoDto()
        //        {
        //            Id = 1,
        //            Title = "栏目一",
        //            Url = "/c/1.html",
        //            KeyWords = "栏目关键字"
        //        }

        //    };

        //    _articleInfoDtos = new List<CmsInfoDto>()
        //    {
        //        new CmsInfoDto()
        //        {
        //            Id = 1,
        //            ColumnInfoId =1,
        //            Title = "文章一",
        //            Content = "文章一内容",
        //            Url = "/a/1.html",
        //            KeyWords = "栏目关键字"
        //        },
        //        new CmsInfoDto()
        //        {
        //            Id = 2,
        //            ColumnInfoId =1,
        //            Title = "文章二",
        //            Content = "文章二内容",
        //            Url = "/abc/2.html",
        //            KeyWords = "栏目关键字"
        //        }

        //    };
        //}

        public IActionResult Index()
        {  
            return View(_columnInfoDtos);
        }

        public IActionResult Article(long cid)
        {
            var articleInfos = _articleInfoDtos.Where(a => a.ColumnInfoId == cid).ToList();
            return View(articleInfos);
        }

        public IActionResult Detail(string url)
        {
            if (_columnInfoDtos.Any(a => a.Url == url))
            {
                return View(_columnInfoDtos.FirstOrDefault(a => a.Url == url));
            }

            if(_articleInfoDtos.Any(a => a.Url == url))
            {
                return View(_articleInfoDtos.FirstOrDefault(a => a.Url == url));
            }

            return NotFound();

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class CmsInfoDto
    {
        public long Id { get; set; }

        public long? ColumnInfoId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Url { get; set; }

        public string KeyWords { get; set; }
    }
}