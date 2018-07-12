using System.Threading.Tasks;

namespace Magicodes.Admin.Security
{
    public interface IPasswordComplexitySettingStore
    {
        Task<PasswordComplexitySetting> GetSettingsAsync();
    }
}
