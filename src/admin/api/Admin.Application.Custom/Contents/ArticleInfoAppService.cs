using Abp.Application.Services.Dto;
using Abp.AspNetZeroCore.Net;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Admin.Application.Custom.Contents.Dto;
using Magicodes.Admin;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Core.Custom.Attachments;
using Magicodes.Admin.Core.Custom.Contents;
using Magicodes.Admin.Dto;
using Magicodes.Admin.Storage;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.Unity.Editor;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Admin.Application.Custom.Contents
{
    /// <summary>
    /// 文章
    /// </summary>
    [AbpAuthorize(AppPermissions.Pages_ArticleInfo)]
    public partial class ArticleInfoAppService : AppServiceBase, IArticleInfoAppService
    {

        private readonly IRepository<ArticleInfo, long> _articleInfoRepository;
        private readonly IExporter _excelExporter;
        private readonly IRepository<ColumnInfo, long> _columnInfoRepository;
        private readonly IRepository<ArticleSourceInfo, long> _articleSourceInfoRepository;
        private readonly EditorHelper _editorHelper;
        private readonly IRepository<ObjectAttachmentInfo, long> _objectAttachmentRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ITempFileCacheManager _tempFileCacheManager;

        /// <summary>
        /// 
        /// </summary>
        public ArticleInfoAppService(
            IRepository<ArticleInfo, long> articleInfoRepository
            , IExporter excelExporter
            , IRepository<ColumnInfo, long> columnInfoRepository
            , IRepository<ArticleSourceInfo, long> articleSourceInfoRepository
            , EditorHelper editorHelper
            , IRepository<ObjectAttachmentInfo, long> objectAttachmentRepository
            , ICacheManager cacheManager
            , ITempFileCacheManager tempFileCacheManager) : base()
        {
            _articleInfoRepository = articleInfoRepository;
            _excelExporter = excelExporter;

            _columnInfoRepository = columnInfoRepository;

            _articleSourceInfoRepository = articleSourceInfoRepository;
            _editorHelper = editorHelper;
            _objectAttachmentRepository = objectAttachmentRepository;
            _cacheManager = cacheManager;
            _tempFileCacheManager = tempFileCacheManager;
        }

        /// <summary>
        /// 获取文章列表
        /// </summary>
        public async Task<PagedResultDto<ArticleInfoListDto>> GetArticleInfos(GetArticleInfosInput input)
        {
            async Task<PagedResultDto<ArticleInfoListDto>> getListFunc(bool isLoadSoftDeleteData)
            {
                var query = CreateArticleInfosQuery(input);

                //仅加载已删除的数据
                if (isLoadSoftDeleteData)
                {
                    query = query.Where(p => p.IsDeleted);
                }

                var resultCount = await query.CountAsync();
                var results = await query
                    .OrderBy(input.Sorting)
                    .PageBy(input)
                    .ToListAsync();

                return new PagedResultDto<ArticleInfoListDto>(resultCount, results.MapTo<List<ArticleInfoListDto>>());
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
        /// 导出文章
        /// </summary>
        public async Task<FileDto> GetArticleInfosToExcel(GetArticleInfosInput input)
        {
            async Task<List<ArticleInfoExportDto>> getListFunc(bool isLoadSoftDeleteData)
            {
                var query = CreateArticleInfosQuery(input);
                var results = await query
                    .OrderBy(input.Sorting)
                    .ToListAsync();

                var exportListDtos = results.MapTo<List<ArticleInfoExportDto>>();
                if (exportListDtos.Count == 0)
                {
                    throw new UserFriendlyException(L("NoDataToExport"));
                }

                return exportListDtos;
            }

            List<ArticleInfoExportDto> exportData = null;

            //是否仅加载回收站数据
            if (input.IsOnlyGetRecycleData)
            {
                using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    exportData = await getListFunc(true);
                }
            }

            exportData = await getListFunc(false);
            var fileDto = new FileDto(L("ArticleInfo") + L("ExportData") + ".xlsx", MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);
            var byteArray = await _excelExporter.ExportAsByteArray(exportData);
            _tempFileCacheManager.SetFile(fileDto.FileToken, byteArray);
            return fileDto;
        }

        /// <summary>
        /// 
        /// </summary>
        private IQueryable<ArticleInfo> CreateArticleInfosQuery(GetArticleInfosInput input)
        {
            var query = _articleInfoRepository.GetAllIncluding(p => p.ColumnInfo, p => p.ArticleSourceInfo);

            //关键字搜索
            query = query
                    .WhereIf(
                    !input.Filter.IsNullOrEmpty(),
                    p => p.Title.Contains(input.Filter) || p.Publisher.Contains(input.Filter) || p.Content.Contains(input.Filter) || p.SeoTitle.Contains(input.Filter) || p.KeyWords.Contains(input.Filter) || p.Introduction.Contains(input.Filter) || p.StaticPageUrl.Contains(input.Filter) || p.Url.Contains(input.Filter));


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
        /// 获取文章
        /// </summary>
        [AbpAuthorize(AppPermissions.Pages_ArticleInfo_Create, AppPermissions.Pages_ArticleInfo_Edit)]
        public async Task<GetArticleInfoForEditOutput> GetArticleInfoForEdit(NullableIdDto<long> input)
        {
            ArticleInfoEditDto editDto;
            if (input.Id.HasValue)
            {
                var info = await _articleInfoRepository.GetAsync(input.Id.Value);
                editDto = info.MapTo<ArticleInfoEditDto>();
            }
            else
            {
                editDto = new ArticleInfoEditDto();

            }
            return new GetArticleInfoForEditOutput
            {
                ArticleInfo = editDto
            };
        }

        /// <summary>
        /// 创建或者编辑文章
        /// </summary>
        [AbpAuthorize(AppPermissions.Pages_ArticleInfo_Create, AppPermissions.Pages_ArticleInfo_Edit)]
        public async Task CreateOrUpdateArticleInfo(CreateOrUpdateArticleInfoDto input)
        {
            //处理Html图片
            if (!string.IsNullOrWhiteSpace(input.ArticleInfo.Content))
            {
                input.ArticleInfo.Content = await _editorHelper.ConvertBase64ImagesForContent(input.ArticleInfo.Content);
            }

            if (!CheckMaxItemCount(input))
            {
                throw new UserFriendlyException(L("ExceedTheMaxCount"));
            }
            if (!input.ArticleInfo.Id.HasValue)
            {
                await CreateArticleInfoAsync(input);
            }
            else
            {
                await UpdateArticleInfoAsync(input);
            }
        }

        /// <summary>
        /// 删除文章
        /// </summary>
        [AbpAuthorize(AppPermissions.Pages_ArticleInfo_Delete)]
        public async Task DeleteArticleInfo(EntityDto<long> input)
        {
            var articleInfo = await _articleInfoRepository.GetAsync(input.Id);
            articleInfo.IsDeleted = true;
            articleInfo.DeleterUserId = AbpSession.GetUserId();
            articleInfo.DeletionTime = Clock.Now;

        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(AppPermissions.Pages_ArticleInfo_Create)]
        protected virtual async Task CreateArticleInfoAsync(CreateOrUpdateArticleInfoDto input)
        {
            if (_articleInfoRepository.GetAll().Any(p => p.Title == input.ArticleInfo.Title))
            {
                throw new UserFriendlyException(L("TitleExist"));
            }
            var articleInfo = new ArticleInfo()
            {
                Code = GetCode(),
                Title = input.ArticleInfo.Title,
                Publisher = input.ArticleInfo.Publisher,
                ColumnInfoId = input.ArticleInfo.ColumnInfoId,
                ArticleSourceInfoId = input.ArticleInfo.ArticleSourceInfoId,
                ReleaseTime = input.ArticleInfo.ReleaseTime,
                Content = input.ArticleInfo.Content,
                IsActive = input.ArticleInfo.IsActive,
                IsNeedAuthorizeAccess = input.ArticleInfo.IsNeedAuthorizeAccess,
                SeoTitle = input.ArticleInfo.SeoTitle,
                KeyWords = input.ArticleInfo.KeyWords,
                Introduction = input.ArticleInfo.Introduction,
                StaticPageUrl = input.ArticleInfo.StaticPageUrl,
                Url = input.ArticleInfo.Url,
                IsStatic = input.ArticleInfo.IsStatic,
                RecommendedType = input.ArticleInfo.RecommendedType,
                CreatorUserId = AbpSession.UserId,
                CreationTime = Clock.Now,
                TenantId = AbpSession.TenantId
            };
            await _articleInfoRepository.InsertAsync(articleInfo);

        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(AppPermissions.Pages_ArticleInfo_Edit)]
        protected virtual async Task UpdateArticleInfoAsync(CreateOrUpdateArticleInfoDto input)
        {
            Debug.Assert(input.ArticleInfo.Id != null, "必须设置input.ArticleInfo.Id的值");

            var articleInfo = await _articleInfoRepository.GetAsync(input.ArticleInfo.Id.Value);

            if (input.ArticleInfo.Title != articleInfo.Title)
            {
                if (_articleInfoRepository.GetAll().Any(p => p.Title == input.ArticleInfo.Title))
                {
                    throw new UserFriendlyException(L("TitleExist"));
                }
            }
            articleInfo.Title = input.ArticleInfo.Title;
            articleInfo.Publisher = input.ArticleInfo.Publisher;
            articleInfo.ColumnInfoId = input.ArticleInfo.ColumnInfoId;
            articleInfo.ArticleSourceInfoId = input.ArticleInfo.ArticleSourceInfoId;
            articleInfo.ReleaseTime = input.ArticleInfo.ReleaseTime;
            articleInfo.Content = input.ArticleInfo.Content;
            articleInfo.IsActive = input.ArticleInfo.IsActive;
            articleInfo.IsNeedAuthorizeAccess = input.ArticleInfo.IsNeedAuthorizeAccess;
            articleInfo.SeoTitle = input.ArticleInfo.SeoTitle;
            articleInfo.KeyWords = input.ArticleInfo.KeyWords;
            articleInfo.Introduction = input.ArticleInfo.Introduction;
            articleInfo.StaticPageUrl = input.ArticleInfo.StaticPageUrl;
            articleInfo.Url = input.ArticleInfo.Url;
            articleInfo.RecommendedType = input.ArticleInfo.RecommendedType;
            articleInfo.IsStatic = input.ArticleInfo.IsStatic;
        }

        /// <summary>
        /// 恢复
        /// </summary>
        [AbpAuthorize(AppPermissions.Pages_ArticleInfo_Restore)]
        public async Task RestoreArticleInfo(long id)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var articleInfo = await _articleInfoRepository.GetAsync(id);
                articleInfo.IsDeleted = false;
                articleInfo.LastModifierUserId = AbpSession.GetUserId();
                articleInfo.LastModificationTime = Clock.Now;
            }
        }



        /// <summary>
        /// 获取选择列表
        /// </summary>
        public async Task<List<GetDataComboItemDto<long>>> GetColumnInfoDataComboItems()
        {
            var list = await _columnInfoRepository.GetAll().Where(a=>a.ColumnType == ColumnTypes.Html)
            //.Where(p => !p.IsActive)
            .OrderByDescending(p => p.Id)
            .Select(p => new { p.Id, p.Title }).ToListAsync();
            return list.Select(p => new GetDataComboItemDto<long>()
            {
                DisplayName = p.Title,
                Value = p.Id
            }).ToList();
        }



        /// <summary>
        /// 获取选择列表
        /// </summary>
        public async Task<List<GetDataComboItemDto<long>>> GetArticleSourceInfoDataComboItems()
        {
            var list = await _articleSourceInfoRepository.GetAll()
            //.Where(p => !p.IsActive)
            .OrderByDescending(p => p.Id)
            .Select(p => new { p.Id, p.Name }).ToListAsync();
            return list.Select(p => new GetDataComboItemDto<long>()
            {
                DisplayName = p.Name,
                Value = p.Id
            }).ToList();
        }

        /// <summary>
        /// IsActive开关服务
        /// </summary>
        /// <param name="input">开关输入参数</param>
        /// <returns></returns>
        [AbpAuthorize(AppPermissions.Pages_ArticleInfo_Edit)]
        public async Task UpdateIsActiveSwitchAsync(SwitchEntityInputDto<long> input)
        {
            var articleInfo = await _articleInfoRepository.GetAsync(input.Id);
            articleInfo.IsActive = input.SwitchValue;
        }

        /// <summary>
        /// IsNeedAuthorizeAccess开关服务
        /// </summary>
        /// <param name="input">开关输入参数</param>
        /// <returns></returns>
        [AbpAuthorize(AppPermissions.Pages_ArticleInfo_Edit)]
        public async Task UpdateIsNeedAuthorizeAccessSwitchAsync(SwitchEntityInputDto<long> input)
        {
            var articleInfo = await _articleInfoRepository.GetAsync(input.Id);
            articleInfo.IsNeedAuthorizeAccess = input.SwitchValue;
        }

        private bool CheckMaxItemCount(CreateOrUpdateArticleInfoDto input)
        {
            var columnInfoMaxItemCount = _columnInfoRepository.Get(input.ArticleInfo.ColumnInfoId).MaxItemCount;
            if (!columnInfoMaxItemCount.HasValue)
            {
                return true;
            }
            var columnInfoCurrentCount = _articleInfoRepository.GetAll().Count(a => a.ColumnInfoId == input.ArticleInfo.ColumnInfoId);
            return columnInfoMaxItemCount > columnInfoCurrentCount;
        }
    }
}