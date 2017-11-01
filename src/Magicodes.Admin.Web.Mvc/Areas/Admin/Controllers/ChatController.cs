using System;
using System.Net;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Magicodes.Admin.Chat;
using Magicodes.Admin.Storage;
using Magicodes.Admin.Web.Controllers;
using Newtonsoft.Json.Linq;

namespace Magicodes.Admin.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AbpMvcAuthorize]
    public class ChatController : ChatControllerBase
    {
        public ChatController(IBinaryObjectManager binaryObjectManager, IChatAppService chatAppService) :
            base(binaryObjectManager, chatAppService)
        {
        }

        public async Task<ActionResult> GetImage(int id, string contentType)
        {
            var message = await ChatAppService.FindMessageAsync(id, AbpSession.GetUserId());
            var jsonMessage = JObject.Parse(message.Message.Substring("[image]".Length));
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                var fileObject = await BinaryObjectManager.GetOrNullAsync(Guid.Parse(((JValue)jsonMessage["id"]).Value.ToString()));
                if (fileObject == null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound);
                }

                return File(fileObject.Bytes, contentType);
            }
        }

        public async Task<ActionResult> GetFile(int id, string contentType)
        {
            var message =await ChatAppService.FindMessageAsync(id, AbpSession.GetUserId());
            var jsonMessage = JObject.Parse(message.Message.Substring("[file]".Length));
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                var fileObject = await BinaryObjectManager.GetOrNullAsync(Guid.Parse(((JValue)jsonMessage["id"]).Value.ToString()));
                if (fileObject == null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound);
                }

                return File(fileObject.Bytes, contentType);
            }
        }
    }
}