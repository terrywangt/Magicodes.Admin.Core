using System.Threading.Tasks;
using Magicodes.Admin.Sessions.Dto;

namespace Magicodes.Admin.Web.Session
{
    public interface IPerRequestSessionCache
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformationsAsync();
    }
}
