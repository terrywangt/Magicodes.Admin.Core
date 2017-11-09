using System;
using System.Net;
using System.Threading.Tasks;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Magicodes.Admin.Chat;
using Magicodes.Admin.Storage;
using Newtonsoft.Json.Linq;

namespace Magicodes.Admin.Web.Controllers
{
    public class ChatController : ChatControllerBase
    {
        public ChatController(IBinaryObjectManager binaryObjectManager, IChatAppService chatAppService) : 
            base(binaryObjectManager, chatAppService)
        {
        }

        public async Task<ActionResult> GetUploadedObject(Guid fileId, string contentType)
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                var fileObject = await BinaryObjectManager.GetOrNullAsync(fileId);
                if (fileObject == null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound);
                }

                return File(fileObject.Bytes, contentType);
            }
        }
    }
}