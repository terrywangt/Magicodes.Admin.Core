using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Admin.Application.Custom.LogInfos.Dto;
using Admin.Application.Custome.LogInfos.Dto;
using Magicodes.Admin.Dto;
using System.Threading.Tasks;

namespace Admin.Application.Custom.LogInfos
{
    /// <summary>
    /// 交易日志
    /// </summary>
    public interface ITransactionLogAppService : IApplicationService
    {
		/// <summary>
		/// 获取交易日志列表
		/// </summary>
        Task<PagedResultDto<TransactionLogListDto>> GetTransactionLogs(GetTransactionLogsInput input);

		/// <summary>
		/// 获取交易日志
		/// </summary>
        Task<GetTransactionLogForEditOutput> GetTransactionLogForEdit(NullableIdDto<long> input);

		/// <summary>
		/// 创建或编辑交易日志
		/// </summary>
        Task CreateOrUpdateTransactionLog(CreateOrUpdateTransactionLogDto input);

		/// <summary>
		/// 删除交易日志
		/// </summary>
        Task DeleteTransactionLog(EntityDto<long> input);

		/// <summary>
		/// 导出交易日志
		/// </summary>
        Task<FileDto> GetTransactionLogsToExcel(GetTransactionLogsInput input);
    }
}