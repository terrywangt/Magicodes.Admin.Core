using System.Threading.Tasks;
using Abp.Application.Services;
using Magicodes.Admin.MultiTenancy.Dto;

namespace Magicodes.Admin.MultiTenancy
{
    public interface ITenantRegistrationAppService: IApplicationService
    {
        Task<RegisterTenantOutput> RegisterTenant(RegisterTenantInput input);
    }
}