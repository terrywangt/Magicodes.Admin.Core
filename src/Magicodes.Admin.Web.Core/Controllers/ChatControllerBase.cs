using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.IO.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Magicodes.Admin.Chat;
using Magicodes.Admin.Storage;
using Newtonsoft.Json.Linq;

namespace Magicodes.Admin.Web.Controllers
{
    public class ChatControllerBase : AdminControllerBase
    {
        protected readonly IBinaryObjectManager BinaryObjectManager;
        protected readonly IChatAppService ChatAppService;

        public ChatControllerBase(IBinaryObjectManager binaryObjectManager, IChatAppService chatAppService)
        {
            BinaryObjectManager = binaryObjectManager;
            ChatAppService = chatAppService;
        }

        [HttpPost]
        [AbpMvcAuthorize]
        public async Task<JsonResult> UploadFile()
        {
            try
            {
                var file = Request.Form.Files.First();

                //Check input
                if (file == null)
                {
                    throw new UserFriendlyException(L("File_Empty_Error"));
                }

                if (file.Length > 10000000) //10MB
                {
                    throw new UserFriendlyException(L("File_SizeLimit_Error"));
                }

                byte[] fileBytes;
                using (var stream = file.OpenReadStream())
                {
                    fileBytes = stream.GetAllBytes();
                }

                var fileObject = new BinaryObject(null, fileBytes);
                using (CurrentUnitOfWork.SetTenantId(null))
                {
                    await BinaryObjectManager.SaveAsync(fileObject);
                }
                return Json(new AjaxResponse(new
                {
                    id = fileObject.Id,
                    name = file.FileName,
                    contentType = file.ContentType
                }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
    }
}