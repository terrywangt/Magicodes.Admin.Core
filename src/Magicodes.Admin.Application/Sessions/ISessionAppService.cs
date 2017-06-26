using System.Threading.Tasks;
using Abp.Application.Services;
using Magicodes.Admin.Sessions.Dto;

namespace Magicodes.Admin.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();

        Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken();
    }
}
