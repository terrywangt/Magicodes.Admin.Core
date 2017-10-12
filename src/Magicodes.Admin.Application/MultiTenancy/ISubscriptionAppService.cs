using System.Threading.Tasks;
using Abp.Application.Services;

namespace Magicodes.Admin.MultiTenancy
{
    public interface ISubscriptionAppService : IApplicationService
    {
        Task UpgradeTenantToEquivalentEdition(int upgradeEditionId);
    }
}
