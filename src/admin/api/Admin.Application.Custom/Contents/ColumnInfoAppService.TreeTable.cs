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
using Magicodes.ExporterAndImporter.Core;
using Abp.AspNetZeroCore.Net;
using Magicodes.Admin.Dto;
using Abp.Domain.Uow;
using Admin.Application.Custom.Contents.Dto;
using Magicodes.Admin.Core.Custom.Contents;

namespace Admin.Application.Custom.Contents
{
    /// <summary>
    /// 栏目
    /// </summary>
    [AbpAuthorize(AppPermissions.Pages_ColumnInfo)]
    public partial class ColumnInfoAppService : AppServiceBase, IColumnInfoAppService
    {

		/// <summary>
        /// 获取栏目 TreeTable列表
        /// </summary>
        public async Task<TreeTableOutputDto<ColumnInfo>> GetChildrenColumnInfos(GetChildrenColumnInfosInput input)
        {           
            var data = await _columnInfoRepository.GetAll().Where(p => p.ParentId == (input.ParentId ?? 0))
                .OrderBy(p => p.SortNo).ToListAsync();
            var output = new TreeTableOutputDto<ColumnInfo>()
            {
                Data = data.Select(p => new TreeTableRowDto<ColumnInfo>()
                {
                    Data = p
                }).ToList()
            };

            foreach (var treeItemDto in output.Data)
            {
                treeItemDto.Children = _columnInfoRepository.GetAll().Where(p => p.ParentId == treeItemDto.Data.Id)
                    .OrderBy(p => p.SortNo)
                    .Select(p => new TreeTableRowDto<ColumnInfo>()
                    {
                        Data = p
                    }).ToList();
            }
            return output;
        }
	}
}