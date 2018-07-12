using System.Threading.Tasks;
using Abp.Application.Services;
using Magicodes.Admin.Install.Dto;

namespace Magicodes.Admin.Install
{
    public interface IInstallAppService : IApplicationService
    {
        Task Setup(InstallDto input);

        AppSettingsJsonDto GetAppSettingsJson();

        CheckDatabaseOutput CheckDatabase();
    }
}