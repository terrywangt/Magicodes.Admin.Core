using System.Threading.Tasks;
using Abp.Configuration;

namespace Magicodes.Admin.Timing
{
    public interface ITimeZoneService
    {
        Task<string> GetDefaultTimezoneAsync(SettingScopes scope, int? tenantId);
    }
}
