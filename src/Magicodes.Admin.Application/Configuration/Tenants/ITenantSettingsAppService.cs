using System.Threading.Tasks;
using Abp.Application.Services;
using Magicodes.Admin.Configuration.Tenants.Dto;

namespace Magicodes.Admin.Configuration.Tenants
{
    public interface ITenantSettingsAppService : IApplicationService
    {
        Task<TenantSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(TenantSettingsEditDto input);

        Task ClearLogo();

        Task ClearCustomCss();
    }
}
