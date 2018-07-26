using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp;
using Abp.UI;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Domain.Repositories;
using Abp.Application.Services.Dto;
using Microsoft.EntityFrameworkCore;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Runtime.Session;
using Abp.Timing;
using Magicodes.Admin.Authorization;
using Admin.Application.Custom.Contents.Dto;
using Magicodes.Admin.Core.Custom.Contents;
using Magicodes.ExporterAndImporter.Core;
using Abp.AspNetZeroCore.Net;
using Magicodes.Admin.Dto;
using Abp.Domain.Uow;
using Admin.Application.Custom;
using Magicodes.Admin.Core.Custom.Attachments;


namespace Admin.Application.Custom.Contents
{
    /// <summary>
    /// 栏目
    /// </summary>
    [AbpAuthorize(AppPermissions.Pages_ColumnInfo)]
    public partial class ColumnInfoAppService : AppServiceBase, IColumnInfoAppService
    {
        private readonly IRepository<ColumnInfo, long> _columnInfoRepository;
		private readonly IRepository<ObjectAttachmentInfo, long> _objectAttachmentRepository;
		private readonly IExporter _excelExporter;

		/// <summary>
		/// 
		/// </summary>
        public ColumnInfoAppService(
            IRepository<ColumnInfo, long> columnInfoRepository 
            , IExporter excelExporter
			, IRepository<ObjectAttachmentInfo, long> objectAttachmentRepository
            ) : base()
        {
            _columnInfoRepository = columnInfoRepository;
			_excelExporter = excelExporter;
			_objectAttachmentRepository = objectAttachmentRepository;
        }

		/// <summary>
		/// 获取栏目列表
		/// </summary>
        public async Task<PagedResultDto<ColumnInfoListDto>> GetColumnInfos(GetColumnInfosInput input)
        {
            async Task<PagedResultDto<ColumnInfoListDto>> getListFunc(bool isLoadSoftDeleteData)
            {
                var query = CreateColumnInfosQuery(input);
                
								//仅加载已删除的数据
				if (isLoadSoftDeleteData)
                query = query.Where(p => p.IsDeleted);
				
				var resultCount = await query.CountAsync();
                var results = await query
                    .OrderBy(input.Sorting)
                    .PageBy(input)
                    .ToListAsync();

				return new PagedResultDto<ColumnInfoListDto>(resultCount, results.MapTo<List<ColumnInfoListDto>>());
            }

            //是否仅加载回收站数据
            if (input.IsOnlyGetRecycleData)
            {
                using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    return await getListFunc(true);
                }
            }
            return await getListFunc(false);
        }

		/// <summary>
		/// 导出栏目
		/// </summary>
		public async Task<FileDto> GetColumnInfosToExcel(GetColumnInfosInput input)
        {
            async Task<List<ColumnInfoExportDto>> getListFunc(bool isLoadSoftDeleteData)
            {
                var query = CreateColumnInfosQuery(input);
                var results = await query
                    .OrderBy(input.Sorting)
                    .ToListAsync();

                var exportListDtos = results.MapTo<List<ColumnInfoExportDto>>();
                if (exportListDtos.Count == 0)
                    throw new UserFriendlyException(L("NoDataToExport"));
                return exportListDtos;
            }

            List<ColumnInfoExportDto> exportData = null;

			            //是否仅加载回收站数据
            if (input.IsOnlyGetRecycleData)
            {
                using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    exportData = await getListFunc(true);
                }
            }
			
            exportData = await getListFunc(false);
            var fileDto = new FileDto(L("ColumnInfo") +L("ExportData")+ ".xlsx", MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);
            var filePath = GetTempFilePath(fileName: fileDto.FileToken);
            await _excelExporter.Export(filePath, exportData);
            return fileDto;
        }

		/// <summary>
		/// 
		/// </summary>
        private IQueryable<ColumnInfo> CreateColumnInfosQuery(GetColumnInfosInput input)
        {
            var query = _columnInfoRepository.GetAll();
			
			//关键字搜索
			query = query
					.WhereIf(
                    !input.Filter.IsNullOrEmpty(),
					p => p.Title.Contains(input.Filter) || p.Description.Contains(input.Filter) || p.Introduction.Contains(input.Filter) || p.IconCls.Contains(input.Filter) || p.Url.Contains(input.Filter));
			
			
			//创建时间范围搜索
			query = query
                .WhereIf(input.CreationDateStart.HasValue, t => t.CreationTime >= input.CreationDateStart.Value)
                .WhereIf(input.CreationDateEnd.HasValue, t => t.CreationTime <= input.CreationDateEnd.Value);
			
			
			//修改时间范围搜索
			query = query
                .WhereIf(input.ModificationTimeStart.HasValue, t => t.LastModificationTime >= input.ModificationTimeStart.Value)
                .WhereIf(input.ModificationTimeEnd.HasValue, t => t.LastModificationTime <= input.ModificationTimeEnd.Value);
			
            return query;
        }

		/// <summary>
		/// 获取栏目
		/// </summary>
        [AbpAuthorize(AppPermissions.Pages_ColumnInfo_Create, AppPermissions.Pages_ColumnInfo_Edit)]
        public async Task<GetColumnInfoForEditOutput> GetColumnInfoForEdit(NullableIdDto<long> input)
        {
            ColumnInfoEditDto editDto;
            if (input.Id.HasValue)
            {
                var info = await _columnInfoRepository.GetAsync(input.Id.Value);
                editDto = info.MapTo<ColumnInfoEditDto>();
            }
            else
            {
                editDto = new ColumnInfoEditDto();

            }
            return new GetColumnInfoForEditOutput
            {
                ColumnInfo = editDto
            };
        }

		/// <summary>
		/// 创建或者编辑栏目
		/// </summary>
        [AbpAuthorize(AppPermissions.Pages_ColumnInfo_Create, AppPermissions.Pages_ColumnInfo_Edit)]
        public async Task CreateOrUpdateColumnInfo(CreateOrUpdateColumnInfoDto input)
        {
            if (!input.ColumnInfo.Id.HasValue)
            {
                await CreateColumnInfoAsync(input);
            }
            else
            {
                await UpdateColumnInfoAsync(input);
            }
        }

		/// <summary>
		/// 删除栏目
		/// </summary>
        [AbpAuthorize(AppPermissions.Pages_ColumnInfo_Delete)]
        public async Task DeleteColumnInfo(EntityDto<long> input)
        {
            var columnInfo = await _columnInfoRepository.GetAsync(input.Id);
            columnInfo.IsDeleted = true;
            columnInfo.DeleterUserId = AbpSession.GetUserId();
            columnInfo.DeletionTime = Clock.Now;
            
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(AppPermissions.Pages_ColumnInfo_Create)]
        protected virtual async Task CreateColumnInfoAsync(CreateOrUpdateColumnInfoDto input)
        {
            if (_columnInfoRepository.GetAll().Any(p => p.Title == input.ColumnInfo.Title))
            {
                throw new UserFriendlyException(L("TitleExist"));
            }
            //如果排序号为空，则自动设置序号
            if (!input.ColumnInfo.SortNo.HasValue)
            {
                input.ColumnInfo.SortNo = _columnInfoRepository.GetAll().DefaultIfEmpty().Max(p => p.SortNo.HasValue ? p.SortNo.Value : 0) + 1;
            }
            var columnInfo = new ColumnInfo()
            {
                Title = input.ColumnInfo.Title,
                SortNo = input.ColumnInfo.SortNo,
                IsActive = input.ColumnInfo.IsActive,
                IsNeedAuthorizeAccess = input.ColumnInfo.IsNeedAuthorizeAccess,
                Description = input.ColumnInfo.Description,
                Introduction = input.ColumnInfo.Introduction,
                IconCls = input.ColumnInfo.IconCls,
                Url = input.ColumnInfo.Url,
                CreatorUserId = AbpSession.UserId,
                CreationTime = Clock.Now,
                TenantId = AbpSession.TenantId
            };
            await _columnInfoRepository.InsertAsync(columnInfo);
             
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(AppPermissions.Pages_ColumnInfo_Edit)]
        protected virtual async Task UpdateColumnInfoAsync(CreateOrUpdateColumnInfoDto input)
        {
            Debug.Assert(input.ColumnInfo.Id != null, "必须设置input.ColumnInfo.Id的值");

            var columnInfo = await _columnInfoRepository.GetAsync(input.ColumnInfo.Id.Value);

            if (input.ColumnInfo.Title != columnInfo.Title)
            {
                if (_columnInfoRepository.GetAll().Any(p => p.Title == input.ColumnInfo.Title))
                {
                    throw new UserFriendlyException(L("TitleExist"));
                }
            }
            columnInfo.Title = input.ColumnInfo.Title;
            columnInfo.SortNo = input.ColumnInfo.SortNo;
            columnInfo.IsActive = input.ColumnInfo.IsActive;
            columnInfo.IsNeedAuthorizeAccess = input.ColumnInfo.IsNeedAuthorizeAccess;
            columnInfo.Description = input.ColumnInfo.Description;
            columnInfo.Introduction = input.ColumnInfo.Introduction;
            columnInfo.IconCls = input.ColumnInfo.IconCls;
            columnInfo.Url = input.ColumnInfo.Url;
        }

        /// <summary>
        /// 恢复
        /// </summary>
        [AbpAuthorize(AppPermissions.Pages_ColumnInfo_Restore)]
        public async Task RestoreColumnInfo(long id)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var columnInfo = await _columnInfoRepository.GetAsync(id);
                columnInfo.IsDeleted = false;
                columnInfo.LastModifierUserId = AbpSession.GetUserId();
                columnInfo.LastModificationTime = Clock.Now;
            }
        }

        /// <summary>
        /// 拖拽排序
        /// </summary>
        /// <param name="input">输入参数</param>
        /// <returns></returns>
        [AbpAuthorize(AppPermissions.Pages_ColumnInfo_Edit)]
        public async Task MoveTo(MoveToInputDto<long> input)
        {
            await base.MoveTo(_columnInfoRepository, input);
        }

		/// <summary>
        /// IsActive开关服务
        /// </summary>
        /// <param name="input">开关输入参数</param>
        /// <returns></returns>
		[AbpAuthorize(AppPermissions.Pages_ColumnInfo_Edit)]
        public async Task UpdateIsActiveSwitchAsync(SwitchEntityInputDto<long> input)
		{
            var columnInfo = await _columnInfoRepository.GetAsync(input.Id);
			columnInfo.IsActive = input.SwitchValue;
		}

		/// <summary>
        /// IsNeedAuthorizeAccess开关服务
        /// </summary>
        /// <param name="input">开关输入参数</param>
        /// <returns></returns>
		[AbpAuthorize(AppPermissions.Pages_ColumnInfo_Edit)]
        public async Task UpdateIsNeedAuthorizeAccessSwitchAsync(SwitchEntityInputDto<long> input)
		{
            var columnInfo = await _columnInfoRepository.GetAsync(input.Id);
			columnInfo.IsNeedAuthorizeAccess = input.SwitchValue;
		}


    }
}