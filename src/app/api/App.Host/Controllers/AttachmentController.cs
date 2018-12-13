using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Timing;
using Abp.UI;
using Abp.Web.Models;
using Magicodes.Admin.Attachments;
using Magicodes.Admin.Dto;
using Magicodes.Storage.Core;
using Magicodes.Unity.Storage;
using Microsoft.AspNetCore.Mvc;

namespace App.Host.Controllers
{
    [Route("api/Attachment")]
    public class AttachmentController : AbpController
    {
        private readonly IRepository<AttachmentInfo, long> _attachmentInfoRepository;
        private readonly IStorageManager _storageManager;

        public AttachmentController(IRepository<AttachmentInfo, long> attachmentInfoRepository, IStorageManager storageManager)
        {
            this._attachmentInfoRepository = attachmentInfoRepository;
            _storageManager = storageManager;
        }

        [HttpPost("UploadFiles")]
        [DisableAuditing]
        public async Task<ActionResult> UploadFiles(AddAttachmentInputDto input)
        {
            var files = Request.Form.Files;
            if (files == null || files.Count == 0)
            {
                throw new UserFriendlyException(L("File_Empty_Error"));
            }
            try
            {
                var filesOutput = new List<AttachmentInfo>();
                foreach (var item in files)
                {
                    if (item == null)
                    {
                        throw new UserFriendlyException(L("文件上传错误!"));
                    }
                    //5M
                    if (item.Length > 5242880)
                    {
                        throw new UserFriendlyException(L("File_SizeLimit_Error"));
                    }

                    if (!Enum.TryParse(input.AttachmentSort.ToString(), false, out Magicodes.Admin.Attachments.AttachmentSorts result))
                    {
                        throw new UserFriendlyException(L("PleaseSelectProperSort"));
                    }

                    try
                    {
                        using (var stream = item.OpenReadStream())
                        {
                            var tempFileName = Guid.NewGuid().ToString("N") + Path.GetExtension(item.FileName);
                            await _storageManager.StorageProvider.SaveBlobStream((AbpSession.TenantId ?? 0).ToString(), tempFileName, stream);
                            var blobInfo = await _storageManager.StorageProvider.GetBlobFileInfo((AbpSession.TenantId ?? 0).ToString(), tempFileName);
                            var attachmentType = AttachmentTypes.File;
                            if (blobInfo.ContentType.StartsWith("video/"))
                            {
                                attachmentType = AttachmentTypes.Video;
                            }
                            else if (blobInfo.ContentType.StartsWith("image/"))
                            {
                                attachmentType = AttachmentTypes.Image;
                            }
                            else if (blobInfo.ContentType.StartsWith("audio/"))
                            {
                                attachmentType = AttachmentTypes.Audio;
                            }
                            var attach = new AttachmentInfo()
                            {
                                ContentType = blobInfo.ContentType,
                                CreationTime = Clock.Now,
                                CreatorUserId = AbpSession.UserId,
                                FileLength = blobInfo.Length,
                                Name = item.FileName,
                                TenantId = AbpSession.TenantId,
                                Url = blobInfo.Url,
                                BlobName = blobInfo.Name,
                                ContainerName = blobInfo.Container,
                                AttachmentType = attachmentType,
                                ContentMD5 = blobInfo.ContentMD5,
                                AttachmentSorts = (Magicodes.Admin.Attachments.AttachmentSorts)input.AttachmentSort
                            };
                            _attachmentInfoRepository.Insert(attach);
                            filesOutput.Add(attach);
                        }
                    }
                    catch (StorageException ex)
                    {
                        throw new UserFriendlyException(message: ex.ProviderMessage);
                    }
                }
                return Json(new AjaxResponse(filesOutput));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
    }
}
