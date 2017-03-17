using System.Threading.Tasks;
using Abp.Domain.Policies;

namespace Magicodes.Admin.Authorization.Users
{
    public interface IUserPolicy : IPolicy
    {
        Task CheckMaxUserCountAsync(int tenantId);
    }
}
