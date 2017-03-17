using System.Threading.Tasks;
using Abp.Domain.Services;

namespace Magicodes.Admin.Authorization.Impersonation
{
    public interface IImpersonationManager : IDomainService
    {
        Task<UserAndIdentity> GetImpersonatedUserAndIdentity(string impersonationToken, string authenticationType);

        Task<string> GetImpoersonateToken(long userId, int? tenantId);

        Task<string> GetBackToImpersonatorToken();
    }
}