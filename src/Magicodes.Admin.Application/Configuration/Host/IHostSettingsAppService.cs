using System.Threading.Tasks;
using Abp.Application.Services;
using Magicodes.Admin.Configuration.Host.Dto;

namespace Magicodes.Admin.Configuration.Host
{
    public interface IHostSettingsAppService : IApplicationService
    {
        Task<HostSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(HostSettingsEditDto input);

        Task SendTestEmail(SendTestEmailInput input);
    }
}
