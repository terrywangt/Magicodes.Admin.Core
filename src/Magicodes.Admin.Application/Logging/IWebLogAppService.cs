using Abp.Application.Services;
using Magicodes.Admin.Dto;
using Magicodes.Admin.Logging.Dto;

namespace Magicodes.Admin.Logging
{
    public interface IWebLogAppService : IApplicationService
    {
        GetLatestWebLogsOutput GetLatestWebLogs();

        FileDto DownloadWebLogs();
    }
}
