using Abp.Auditing;
using Microsoft.AspNetCore.Mvc;
using Magicodes.Admin.Dto;
using Magicodes.Admin.Storage;

namespace Magicodes.Admin.Web.Controllers
{
    public class FileController : AdminControllerBase
    {
        private readonly ITempFileCacheManager _tempFileCacheManager;

        public FileController(ITempFileCacheManager tempFileCacheManager)
        {
            _tempFileCacheManager = tempFileCacheManager;
        }

        [DisableAuditing]
        public ActionResult DownloadTempFile(FileDto file)
        {
            var fileBytes = _tempFileCacheManager.GetFile(file.FileToken);
            if (fileBytes == null)
            {
                return NotFound(L("RequestedFileDoesNotExists"));
            }

            return File(fileBytes, file.FileType, file.FileName);
        }
    }
}