using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Reflection.Extensions;
using Admin.Application.Custom.Common.Dto;
using Magicodes.Admin.Core.Custom;
using Magicodes.Admin.Core.Custom.Attachments;
using Magicodes.Admin.Core.Custom.LogInfos;
using Microsoft.EntityFrameworkCore;

namespace Admin.Application.Custom.Common
{
    /// <summary>
    /// 通用服务
    /// </summary>
    public class CommonAppService : AppServiceBase, ICommonAppService
    {
        private readonly IRepository<ObjectAttachmentInfo, long> _objectAttachmentInfoRepository;
        private readonly ISettingManager _settingManager;
        private readonly IRepository<TransactionLog, long> _transactionLogRepository;

        public CommonAppService(
            IRepository<ObjectAttachmentInfo, long> objectAttachmentInfoRepository
            , ISettingManager settingManager
            , IRepository<TransactionLog, long> transactionLogRepository)
        {
            _objectAttachmentInfoRepository = objectAttachmentInfoRepository;
            _settingManager = settingManager;
            _transactionLogRepository = transactionLogRepository;
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
                .Where(p => p.ObjectId == input.ObjectId && p.ObjectType == objectType && p.AttachmentInfo.AttachmentType == AttachmentTypes.Image)
                .Select(p=>p.AttachmentInfo)
                .ToListAsync();
            return list.MapTo<List<GetObjectImagesListDto>>();
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
    }
}
