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

using Magicodes.Admin.Core.Custom.Contents;

namespace Admin.Application.Custom.Contents
{
    /// <summary>
    /// 
    /// </summary>
    [AbpAuthorize(AppPermissions.Pages_ArticleInfo_ArticleTagInfo)]
    public partial class ArticleInfoArticleTagInfoAppService : AppServiceBase, IArticleInfoArticleTagInfoAppService
    {
        private readonly IRepository<ArticleTagInfo, long> _articleTagInfoRepository;
        private readonly IRepository<ArticleInfo, long> _articleInfoRepository;
    		private readonly IExporter _excelExporter;

		/// <summary>
		/// 
		/// </summary>
        public ArticleInfoArticleTagInfoAppService(
            IRepository<ArticleTagInfo, long> articleTagInfoRepository 
            , IExporter excelExporter
            , IRepository<ArticleInfo, long> articleInfoRepository
            ) : base()
        {
            _articleTagInfoRepository = articleTagInfoRepository;
			_excelExporter = excelExporter;
			
            _articleInfoRepository = articleInfoRepository;
        }

		/// <summary>
		/// 获取列表
		/// </summary>
        public async Task<PagedResultDto<ArticleTagInfoListDto>> GetArticleTagInfos(GetArticleInfoArticleTagInfosInput input)
        {
            async Task<PagedResultDto<ArticleTagInfoListDto>> getListFunc(bool isLoadSoftDeleteData)
            {
                var query = CreateArticleTagInfosQuery(input);
                
								//仅加载已删除的数据
				if (isLoadSoftDeleteData)
                query = query.Where(p => p.IsDeleted);
				
				var resultCount = await query.CountAsync();
                var results = await query
                    .OrderBy(input.Sorting)
                    .PageBy(input)
                    .ToListAsync();

				return new PagedResultDto<ArticleTagInfoListDto>(resultCount, results.MapTo<List<ArticleTagInfoListDto>>());
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
		/// 导出
		/// </summary>
		public async Task<FileDto> GetArticleTagInfosToExcel(GetArticleInfoArticleTagInfosInput input)
        {
            async Task<List<ArticleTagInfoExportDto>> getListFunc(bool isLoadSoftDeleteData)
            {
                var query = CreateArticleTagInfosQuery(input);
                var results = await query
                    .OrderBy(input.Sorting)
                    .ToListAsync();

                var exportListDtos = results.MapTo<List<ArticleTagInfoExportDto>>();
                if (exportListDtos.Count == 0)
                    throw new UserFriendlyException(L("NoDataToExport"));
                return exportListDtos;
            }

            List<ArticleTagInfoExportDto> exportData = null;

			            //是否仅加载回收站数据
            if (input.IsOnlyGetRecycleData)
            {
                using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    exportData = await getListFunc(true);
                }
            }
			
            exportData = await getListFunc(false);
            var fileDto = new FileDto(L("ArticleTagInfo") +L("ExportData")+ ".xlsx", MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);
            var filePath = GetTempFilePath(fileName: fileDto.FileToken);
            await _excelExporter.Export(filePath, exportData);
            return fileDto;
        }

		/// <summary>
		/// 
		/// </summary>
        private IQueryable<ArticleTagInfo> CreateArticleTagInfosQuery(GetArticleInfoArticleTagInfosInput input)
        {
            var query = _articleTagInfoRepository.GetAllIncluding(p=>p.ArticleInfo).Where(p => p.ArticleInfo.Id == input.ArticleInfoId);;

			
			//关键字搜索
			query = query
					.WhereIf(
                    !input.Filter.IsNullOrEmpty(),
					p => p.Name.Contains(input.Filter));
			
			
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
		/// 获取
		/// </summary>
        [AbpAuthorize(AppPermissions.Pages_ArticleInfo_ArticleTagInfo_Create, AppPermissions.Pages_ArticleInfo_ArticleTagInfo_Edit)]
        public async Task<GetArticleTagInfoForEditOutput> GetArticleTagInfoForEdit(NullableIdDto<long> input)
        {
            ArticleTagInfoEditDto editDto;
            if (input.Id.HasValue)
            {
                var info = await _articleTagInfoRepository.GetAsync(input.Id.Value);
                editDto = info.MapTo<ArticleTagInfoEditDto>();
            }
            else
            {
                editDto = new ArticleTagInfoEditDto();

            }
            return new GetArticleTagInfoForEditOutput
            {
                ArticleTagInfo = editDto
            };
        }

		/// <summary>
		/// 创建或者编辑
		/// </summary>
        [AbpAuthorize(AppPermissions.Pages_ArticleInfo_ArticleTagInfo_Create, AppPermissions.Pages_ArticleInfo_ArticleTagInfo_Edit)]
        public async Task CreateOrUpdateArticleTagInfo(CreateOrUpdateArticleInfoArticleTagInfoDto input)
        {
            if (!input.ArticleTagInfo.Id.HasValue)
            {
                await CreateArticleTagInfoAsync(input);
            }
            else
            {
                await UpdateArticleTagInfoAsync(input);
            }
        }

		/// <summary>
		/// 删除
		/// </summary>
        [AbpAuthorize(AppPermissions.Pages_ArticleInfo_ArticleTagInfo_Delete)]
        public async Task DeleteArticleTagInfo(EntityDto<long> input)
        {
            var articleTagInfo = await _articleTagInfoRepository.GetAsync(input.Id);
            articleTagInfo.IsDeleted = true;
            articleTagInfo.DeleterUserId = AbpSession.GetUserId();
            articleTagInfo.DeletionTime = Clock.Now;
            
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(AppPermissions.Pages_ArticleInfo_ArticleTagInfo_Create)]
        protected virtual async Task CreateArticleTagInfoAsync(CreateOrUpdateArticleInfoArticleTagInfoDto input)
        {
            if (_articleTagInfoRepository.GetAll().Any(p => p.Name == input.ArticleTagInfo.Name))
            {
                throw new UserFriendlyException(L("NameExist"));
            }
            var articleTagInfo = new ArticleTagInfo()
            {
                ArticleInfoId = input.ArticleTagInfo.ArticleInfoId,
                Name = input.ArticleTagInfo.Name,
                CreatorUserId = AbpSession.UserId,
                CreationTime = Clock.Now,
                TenantId = AbpSession.TenantId
            };
            await _articleTagInfoRepository.InsertAsync(articleTagInfo);
             
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(AppPermissions.Pages_ArticleInfo_ArticleTagInfo_Edit)]
        protected virtual async Task UpdateArticleTagInfoAsync(CreateOrUpdateArticleInfoArticleTagInfoDto input)
        {
            Debug.Assert(input.ArticleTagInfo.Id != null, "必须设置input.ArticleTagInfo.Id的值");

            var articleTagInfo = await _articleTagInfoRepository.GetAsync(input.ArticleTagInfo.Id.Value);

            if (input.ArticleTagInfo.Name != articleTagInfo.Name)
            {
                if (_articleTagInfoRepository.GetAll().Any(p => p.Name == input.ArticleTagInfo.Name))
                {
                    throw new UserFriendlyException(L("NameExist"));
                }
            }
            articleTagInfo.ArticleInfoId = input.ArticleTagInfo.ArticleInfoId;
            articleTagInfo.Name = input.ArticleTagInfo.Name;
        }

        /// <summary>
        /// 恢复
        /// </summary>
        [AbpAuthorize(AppPermissions.Pages_ArticleInfo_ArticleTagInfo_Restore)]
        public async Task RestoreArticleTagInfo(long id)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var articleTagInfo = await _articleTagInfoRepository.GetAsync(id);
                articleTagInfo.IsDeleted = false;
                articleTagInfo.LastModifierUserId = AbpSession.GetUserId();
                articleTagInfo.LastModificationTime = Clock.Now;
            }
        }



		/// <summary>
		/// 获取选择列表
		/// </summary>
        public async Task<List<GetDataComboItemDto<long>>> GetArticleInfoDataComboItems()
        {
            var list = await _articleInfoRepository.GetAll()
            //.Where(p => !p.IsActive)
            .OrderByDescending(p => p.Id)
            .Select(p => new { p.Id, p.Title }).ToListAsync();
            return list.Select(p => new GetDataComboItemDto<long>()
            {
                DisplayName = p.Title,
                Value = p.Id
            }).ToList();
        }


    }
}