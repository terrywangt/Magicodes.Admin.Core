using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AspNetZeroCore.Net;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.UI;
using Magicodes.Admin;
using Magicodes.Admin.Dto;
using Magicodes.Admin.Storage;
using Magicodes.ExporterAndImporter.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Admin.Application.Custom
{
    /// <summary>
    /// 自定义增删查改导出服务基类
    /// 增
    /// 删
    /// 改
    /// 查
    /// 导出
    /// </summary>
    /// <typeparam name="TEntity">实体模型</typeparam>
    /// <typeparam name="TEntityDto">实体列表Dto</typeparam>
    /// <typeparam name="TPrimaryKey">主键</typeparam>
    /// <typeparam name="TGetAllInput">查询所有Input</typeparam>
    /// <typeparam name="TCreateInput">创建Input</typeparam>
    /// <typeparam name="TUpdateInput">修改Input</typeparam>
    /// <typeparam name="TExportDto">导出Dto</typeparam>
    [AbpAuthorize]
    public abstract class CustomCrudeServiceBase<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TExportDto> : AsyncCrudAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TCreateInput : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
        where TGetAllInput : IPagedAndSortedResultRequest
        where TExportDto : class

    {
        /// <summary>
        /// 目录
        /// </summary>
        public IAppFolders AppFolders { get; set; }

        /// <summary>
        /// 恢复权限
        /// </summary>
        public string RestorePermissionName { get; set; }

        /// <summary>
        /// 导出权限
        /// </summary>
        public string ExportPermissionName { get; set; }

        /// <summary>
        /// Excel导出注入
        /// </summary>
        public IExporter ExcelExporter { get; set; }

        /// <summary>
        /// 临时文件管理器
        /// </summary>
        public ITempFileCacheManager TempFileCacheManager { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        protected CustomCrudeServiceBase(IRepository<TEntity, TPrimaryKey> repository) : base(repository)
        {
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FileDto> ToExcel(TGetAllInput input)
        {
            CheckPermission(ExportPermissionName);
            List<TExportDto> exportData = null;
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var query = CreateFilteredQuery(input);
                var results = await query
                    .OrderBy(input.Sorting)
                    .ToListAsync();

                exportData = results.MapTo<List<TExportDto>>();
                if (exportData.Count == 0)
                {
                    throw new UserFriendlyException(L("NoDataToExport"));
                }
            }
            var fileDto = new FileDto(L(typeof(TEntity).Name) + L("ExportData") + ".xlsx", MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);
            var byteArray = await ExcelExporter.ExportAsByteArray(exportData);
            TempFileCacheManager.SetFile(fileDto.FileToken, byteArray);
            return fileDto;
        }
    }
}