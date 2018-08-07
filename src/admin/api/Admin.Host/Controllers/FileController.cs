using Abp.Auditing;
using Abp.Runtime.Caching;
using Microsoft.AspNetCore.Mvc;
using Magicodes.Admin.Dto;

namespace Magicodes.Admin.Web.Controllers
{
    public class FileController : AdminControllerBase
    {
        private readonly IAppFolders _appFolders;
        private readonly ICacheManager _cacheManager;

        public FileController(IAppFolders appFolders, ICacheManager cacheManager)
        {
            _appFolders = appFolders;
            _cacheManager = cacheManager;
        }

        [DisableAuditing]
        public ActionResult DownloadTempFile(FileDto file)
        {
            var fileBytes = _cacheManager.GetCache(AppConsts.ExcelFileCacheName).Get(file.FileName, ep => ep) as byte[];

            if (fileBytes == null)
            {
                return NotFound(L("RequestedFileDoesNotExists"));
            }

            _cacheManager.GetCache(AppConsts.ExcelFileCacheName).Remove(file.FileName);
            return File(fileBytes, file.FileType, file.FileName);
        }
    }
}