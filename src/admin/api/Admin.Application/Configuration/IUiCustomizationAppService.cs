using System.Threading.Tasks;
using Abp.Application.Services;
using Magicodes.Admin.Configuration.Dto;

namespace Magicodes.Admin.Configuration
{
    public interface IUiCustomizationSettingsAppService : IApplicationService
    {
        Task<UiCustomizationSettingsEditDto> GetUiManagementSettings();

        Task UpdateUiManagementSettings(UiCustomizationSettingsEditDto settings);

        Task UpdateDefaultUiManagementSettings(UiCustomizationSettingsEditDto settings);

        Task UseSystemDefaultSettings();
    }
}
