using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Contents.Dto;
using Magicodes.Admin.Dto;

namespace Magicodes.Admin.Contents
{
	/// <summary>
	/// 栏目
	/// </summary>
    public interface IColumnInfoAppService : IApplicationService
    {
		/// <summary>
		/// 获取栏目列表
		/// </summary>
        Task<PagedResultDto<ColumnInfoListDto>> GetColumnInfos(GetColumnInfosInput input);

		/// <summary>
		/// 获取栏目
		/// </summary>
        Task<GetColumnInfoForEditOutput> GetColumnInfoForEdit(NullableIdDto<long> input);

		/// <summary>
		/// 创建或编辑栏目
		/// </summary>
        Task CreateOrUpdateColumnInfo(CreateOrUpdateColumnInfoDto input);

		/// <summary>
		/// 删除栏目
		/// </summary>
        Task DeleteColumnInfo(EntityDto<long> input);

		/// <summary>
		/// 导出栏目
		/// </summary>
        Task<FileDto> GetColumnInfosToExcel(GetColumnInfosInput input);

        /// <summary>
        /// 删除所有
        /// </summary>
        /// <returns></returns>
        Task DeleteAll();
    }
}