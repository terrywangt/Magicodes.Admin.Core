using Microsoft.Extensions.Configuration;

namespace Magicodes.Admin.Configuration
{
    public interface IAppConfigurationAccessor
    {
        IConfigurationRoot Configuration { get; }
    }
}
