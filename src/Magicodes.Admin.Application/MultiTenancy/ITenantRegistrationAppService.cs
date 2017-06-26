using System.Threading.Tasks;
using Abp.Application.Services;
using Magicodes.Admin.Editions.Dto;
using Magicodes.Admin.MultiTenancy.Dto;

namespace Magicodes.Admin.MultiTenancy
{
    public interface ITenantRegistrationAppService: IApplicationService
    {
        Task<RegisterTenantOutput> RegisterTenant(RegisterTenantInput input);

        Task<EditionsSelectOutput> GetEditionsForSelect();

        Task<EditionSelectDto> GetEdition(int editionId);
    }
}