using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Auditing.Dto;
using Magicodes.Admin.Dto;

namespace Magicodes.Admin.Auditing
{
    public interface IAuditLogAppService : IApplicationService
    {
        Task<PagedResultDto<AuditLogListDto>> GetAuditLogs(GetAuditLogsInput input);

        Task<FileDto> GetAuditLogsToExcel(GetAuditLogsInput input);
    }
}