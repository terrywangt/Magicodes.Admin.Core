using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Contents.Dto;
using Magicodes.Admin.Dto;
using Microsoft.EntityFrameworkCore;

namespace Magicodes.Admin.Contents
{
    /// <summary>
    /// 栏目
    /// </summary>
    //[AbpAuthorize(AppPermissions.Pages_ColumnInfo)]
    public partial class ColumnInfoAppService : AppServiceBase, IColumnInfoAppService
    {

        /// <summary>
        /// 获取栏目 TreeTable列表
        /// </summary>
        [AbpAllowAnonymous]
        public async Task<TreeTableOutputDto<ColumnInfo>> GetChildrenColumnInfos(GetChildrenColumnInfosInput input)
        {           
            var data = await _columnInfoRepository.GetAll()
                .Where(p => p.ParentId == (input.ParentId ?? 0))
                .Where(p=>p.IsNav==input.IsNav)
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
                    .Where(p => p.IsNav == input.IsNav)
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