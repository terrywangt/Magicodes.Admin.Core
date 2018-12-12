using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Dto;
using Magicodes.Admin.LogInfos.Dto;

namespace Magicodes.Admin.LogInfos
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
		/// 删除交易日志
		/// </summary>
        Task DeleteTransactionLog(EntityDto<long> input);

		/// <summary>
		/// 导出交易日志
		/// </summary>
        Task<FileDto> GetTransactionLogsToExcel(GetTransactionLogsInput input);
    }
}