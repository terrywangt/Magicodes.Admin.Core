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


namespace Admin.Application.Custom.Contents
{
    /// <summary>
    /// 文章来源
    /// </summary>
    [AbpAuthorize(AppPermissions.Pages_ArticleSourceInfo)]
    public partial class ArticleSourceInfoAppService : AppServiceBase, IArticleSourceInfoAppService
    {

        private readonly IRepository<ArticleSourceInfo, long> _articleSourceInfoRepository;
	    private readonly IExporter _excelExporter;
    
		/// <summary>
		/// 
		/// </summary>
        public ArticleSourceInfoAppService(
            IRepository<ArticleSourceInfo, long> articleSourceInfoRepository 
            , IExporter excelExporter
            ) : base()
        {
            _articleSourceInfoRepository = articleSourceInfoRepository;
			_excelExporter = excelExporter;

        }

		/// <summary>
		/// 获取文章来源列表
		/// </summary>
        public async Task<PagedResultDto<ArticleSourceInfoListDto>> GetArticleSourceInfos(GetArticleSourceInfosInput input)
        {
            async Task<PagedResultDto<ArticleSourceInfoListDto>> getListFunc(bool isLoadSoftDeleteData)
            {
                var query = CreateArticleSourceInfosQuery(input);
                
								//仅加载已删除的数据
				if (isLoadSoftDeleteData)
                query = query.Where(p => p.IsDeleted);
				
				var resultCount = await query.CountAsync();
                var results = await query
                    .OrderBy(input.Sorting)
                    .PageBy(input)
                    .ToListAsync();

				return new PagedResultDto<ArticleSourceInfoListDto>(resultCount, results.MapTo<List<ArticleSourceInfoListDto>>());
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
		/// 导出文章来源
		/// </summary>
		public async Task<FileDto> GetArticleSourceInfosToExcel(GetArticleSourceInfosInput input)
        {
            async Task<List<ArticleSourceInfoExportDto>> getListFunc(bool isLoadSoftDeleteData)
            {
                var query = CreateArticleSourceInfosQuery(input);
                var results = await query
                    .OrderBy(input.Sorting)
                    .ToListAsync();

                var exportListDtos = results.MapTo<List<ArticleSourceInfoExportDto>>();
                if (exportListDtos.Count == 0)
                    throw new UserFriendlyException(L("NoDataToExport"));
                return exportListDtos;
            }

            List<ArticleSourceInfoExportDto> exportData = null;

			            //是否仅加载回收站数据
            if (input.IsOnlyGetRecycleData)
            {
                using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    exportData = await getListFunc(true);
                }
            }
			
            exportData = await getListFunc(false);
            var fileDto = new FileDto(L("ArticleSourceInfo") + L("ExportData") + ".xlsx", MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);
            var filePath = GetTempFilePath(fileName: fileDto.FileToken);
            await _excelExporter.Export(filePath, exportData);
            return fileDto;
        }

		/// <summary>
		/// 
		/// </summary>
        private IQueryable<ArticleSourceInfo> CreateArticleSourceInfosQuery(GetArticleSourceInfosInput input)
        {
            var query = _articleSourceInfoRepository.GetAll();
			
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
		/// 获取文章来源
		/// </summary>
        [AbpAuthorize(AppPermissions.Pages_ArticleSourceInfo_Create, AppPermissions.Pages_ArticleSourceInfo_Edit)]
        public async Task<GetArticleSourceInfoForEditOutput> GetArticleSourceInfoForEdit(NullableIdDto<long> input)
        {
            ArticleSourceInfoEditDto editDto;
            if (input.Id.HasValue)
            {
                var info = await _articleSourceInfoRepository.GetAsync(input.Id.Value);
                editDto = info.MapTo<ArticleSourceInfoEditDto>();
            }
            else
            {
                editDto = new ArticleSourceInfoEditDto();

            }
            return new GetArticleSourceInfoForEditOutput
            {
                ArticleSourceInfo = editDto
            };
        }

		/// <summary>
		/// 创建或者编辑文章来源
		/// </summary>
        [AbpAuthorize(AppPermissions.Pages_ArticleSourceInfo_Create, AppPermissions.Pages_ArticleSourceInfo_Edit)]
        public async Task CreateOrUpdateArticleSourceInfo(CreateOrUpdateArticleSourceInfoDto input)
        {
            if (!input.ArticleSourceInfo.Id.HasValue)
            {
                await CreateArticleSourceInfoAsync(input);
            }
            else
            {
                await UpdateArticleSourceInfoAsync(input);
            }
        }

		/// <summary>
		/// 删除文章来源
		/// </summary>
        [AbpAuthorize(AppPermissions.Pages_ArticleSourceInfo_Delete)]
        public async Task DeleteArticleSourceInfo(EntityDto<long> input)
        {
            var articleSourceInfo = await _articleSourceInfoRepository.GetAsync(input.Id);
            articleSourceInfo.IsDeleted = true;
            articleSourceInfo.DeleterUserId = AbpSession.GetUserId();
            articleSourceInfo.DeletionTime = Clock.Now;
            
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(AppPermissions.Pages_ArticleSourceInfo_Create)]
        protected virtual async Task CreateArticleSourceInfoAsync(CreateOrUpdateArticleSourceInfoDto input)
        {
            if (_articleSourceInfoRepository.GetAll().Any(p => p.Name == input.ArticleSourceInfo.Name))
            {
                throw new UserFriendlyException(L("NameExist"));
            }
            var articleSourceInfo = new ArticleSourceInfo()
            {
                Name = input.ArticleSourceInfo.Name,
                CreatorUserId = AbpSession.UserId,
                CreationTime = Clock.Now,
                TenantId = AbpSession.TenantId
            };
            await _articleSourceInfoRepository.InsertAsync(articleSourceInfo);
             
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(AppPermissions.Pages_ArticleSourceInfo_Edit)]
        protected virtual async Task UpdateArticleSourceInfoAsync(CreateOrUpdateArticleSourceInfoDto input)
        {
            Debug.Assert(input.ArticleSourceInfo.Id != null, "必须设置input.ArticleSourceInfo.Id的值");

            var articleSourceInfo = await _articleSourceInfoRepository.GetAsync(input.ArticleSourceInfo.Id.Value);

            if (input.ArticleSourceInfo.Name != articleSourceInfo.Name)
            {
                if (_articleSourceInfoRepository.GetAll().Any(p => p.Name == input.ArticleSourceInfo.Name))
                {
                    throw new UserFriendlyException(L("NameExist"));
                }
            }
            articleSourceInfo.Name = input.ArticleSourceInfo.Name;
        }

        /// <summary>
        /// 恢复
        /// </summary>
        [AbpAuthorize(AppPermissions.Pages_ArticleSourceInfo_Restore)]
        public async Task RestoreArticleSourceInfo(long id)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var articleSourceInfo = await _articleSourceInfoRepository.GetAsync(id);
                articleSourceInfo.IsDeleted = false;
                articleSourceInfo.LastModifierUserId = AbpSession.GetUserId();
                articleSourceInfo.LastModificationTime = Clock.Now;
            }
        }


    }
}