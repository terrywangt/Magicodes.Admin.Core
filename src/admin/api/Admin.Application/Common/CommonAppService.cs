using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Reflection.Extensions;
using Abp.UI;
using Magicodes.Admin.Attachments;
using Magicodes.Admin.Common.Dto;
using Magicodes.Admin.Contents;
using Magicodes.Admin.Core.Custom;
using Magicodes.Admin.LogInfos;
using Microsoft.EntityFrameworkCore;

namespace Magicodes.Admin.Common
{
    /// <summary>
    /// 通用服务
    /// </summary>
    public class CommonAppService : AppServiceBase, ICommonAppService
    {
        private readonly IRepository<ObjectAttachmentInfo, long> _objectAttachmentInfoRepository;
        private readonly ISettingManager _settingManager;
        private readonly IRepository<TransactionLog, long> _transactionLogRepository;
        private readonly IRepository<ColumnInfo, long> _columnInfoRepository;


        public CommonAppService(
            IRepository<ObjectAttachmentInfo, long> objectAttachmentInfoRepository
            , ISettingManager settingManager
            , IRepository<TransactionLog, long> transactionLogRepository, 
            IRepository<ColumnInfo, long> columnInfoRepository)
        {
            _objectAttachmentInfoRepository = objectAttachmentInfoRepository;
            _settingManager = settingManager;
            _transactionLogRepository = transactionLogRepository;
            _columnInfoRepository = columnInfoRepository;
        }

        /// <summary>
        /// 获取枚举值列表
        /// </summary>
        /// <returns></returns>
        public List<GetEnumValuesListDto> GetEnumValuesList(GetEnumValuesListInput input)
        {
            var type = typeof(AppCoreModule).GetAssembly().GetType(input.FullName);
            if (!type.IsEnum) return null;
            var names = Enum.GetNames(type);
            var values = Enum.GetValues(type);
            var list = new List<GetEnumValuesListDto>();
            var index = 0;
            foreach (var value in values)
            {
                list.Add(new GetEnumValuesListDto()
                {
                    DisplayName = L(names[index]),
                    Value = Convert.ToInt32(value)
                });
                index++;
            }
            return list;
        }

        /// <summary>
        /// 获取对象图片列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<GetObjectImagesListDto>> GetObjectImages(GetObjectImagesInput input)
        {
            var objectType = Enum.Parse<AttachmentObjectTypes>(input.ObjectType);
            var list = await _objectAttachmentInfoRepository.GetAllIncluding(p => p.AttachmentInfo)
                .Where(p => p.ObjectId == input.ObjectId && p.ObjectType == objectType &&
                            p.AttachmentInfo.AttachmentType == AttachmentTypes.Image)
                .Select(p => new GetObjectImagesListDto
                {
                    Id = p.AttachmentInfo.Id,
                    Name = p.AttachmentInfo.Name,
                    FileLength = p.AttachmentInfo.FileLength,
                    Url = p.AttachmentInfo.Url,
                    IsCover = p.IsCover
                }).ToListAsync();
            return list;
        }

        /// <summary>
        /// 获取对象封面图片
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<GetObjectImagesListDto> GetObjectCoverImage(SetCoverInputDto input)
        {
            var objectType = Enum.Parse<AttachmentObjectTypes>(input.ObjectType);
            var objectAttachmentInfo = await _objectAttachmentInfoRepository.GetAllIncluding(p => p.AttachmentInfo)
                .Where(p =>
                    p.ObjectId == input.ObjectId && p.ObjectType == objectType &&
                    p.AttachmentInfo.AttachmentType == AttachmentTypes.Image)
                .FirstOrDefaultAsync(a => a.IsCover);
            if (objectAttachmentInfo == null)
            {
                return  new GetObjectImagesListDto();
            }
            return new GetObjectImagesListDto
            {
                Id = objectAttachmentInfo.AttachmentInfo.Id,
                Name = objectAttachmentInfo.AttachmentInfo.Name,
                FileLength = objectAttachmentInfo.AttachmentInfo.FileLength,
                Url = objectAttachmentInfo.AttachmentInfo.Url,
                IsCover = objectAttachmentInfo.IsCover
            };
        }

        /// <summary>
        /// 移除对象附件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task RemoveObjectAttachments(RemoveObjectAttachmentsInput input)
        {
            var objectType = Enum.Parse<AttachmentObjectTypes>(input.ObjectType);
            await _objectAttachmentInfoRepository.DeleteAsync(p => input.Ids.Contains(p.AttachmentInfoId) && p.ObjectType == objectType);
        }

        /// <summary>
        /// 添加附件关联
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task AddObjectAttachmentInfos(AddObjectAttachmentInfosInput input)
        {
            var objectType = Enum.Parse<AttachmentObjectTypes>(input.ObjectType);
            if (objectType == AttachmentObjectTypes.ColumnInfo)
            {
                if (!CheckMaxItemCount(input))
                {
                    throw new UserFriendlyException(L("ExceedTheMaxCount"));
                }
            }
            var attachmentInfos = await _objectAttachmentInfoRepository.GetAll().Where(p => p.ObjectId == input.ObjectId && p.ObjectType == objectType).ToListAsync();
            var objectAttachmentInfos = input.AttachmentInfoIds.Select(p => new ObjectAttachmentInfo
            {
                ObjectType = objectType,
                ObjectId = input.ObjectId,
                AttachmentInfoId = p
            }).ToList();
            foreach (var objectAttachmentInfo in objectAttachmentInfos)
            {
                if (attachmentInfos == null || attachmentInfos.Count == 0 || (attachmentInfos.All(p => p.AttachmentInfoId != objectAttachmentInfo.AttachmentInfoId)))
                    await _objectAttachmentInfoRepository.InsertAsync(objectAttachmentInfo);
            }
        }

        /// <summary>
        /// 设置封面
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public async Task SetCover(SetCoverInputDto input)
        {
            var objectType = Enum.Parse<AttachmentObjectTypes>(input.ObjectType);
            var objectAttachmentInfos = await _objectAttachmentInfoRepository.GetAllIncluding(p => p.AttachmentInfo)
                .Where(p =>
                    p.ObjectId == input.ObjectId && p.ObjectType == objectType &&
                    p.AttachmentInfo.AttachmentType == AttachmentTypes.Image)
                .ToListAsync();

            var objectAttachment = objectAttachmentInfos.FirstOrDefault(a => a.IsCover);
            if (objectAttachment != null)
            {
                objectAttachment.IsCover = false;
            }
            var setObjectAttachment =
                objectAttachmentInfos.FirstOrDefault(a => a.AttachmentInfo.Url == input.AttachmentUrl);
            if (setObjectAttachment == null)
            {
                throw new UserFriendlyException();
            }
            setObjectAttachment.IsCover = true;
        }

        private bool CheckMaxItemCount(AddObjectAttachmentInfosInput input)
        {
            var columnInfoMaxItemCount = _columnInfoRepository.Get(input.ObjectId).MaxItemCount;
            if (!columnInfoMaxItemCount.HasValue)
            {
                return true;
            }
            var columnInfoCurrentCount = _objectAttachmentInfoRepository.GetAll().Count(a => a.ObjectId == input.ObjectId);
            return columnInfoMaxItemCount > columnInfoCurrentCount;
        }
    }
}
