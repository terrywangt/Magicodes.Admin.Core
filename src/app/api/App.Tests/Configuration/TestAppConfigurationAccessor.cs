using System.IO;
using Abp.Dependency;
using Magicodes.Admin.Configuration;
using Microsoft.Extensions.Configuration;

namespace App.Tests.Configuration
{
    public class TestAppConfigurationAccessor : IAppConfigurationAccessor, ISingletonDependency
    {
        public IConfigurationRoot Configuration { get; }

        public TestAppConfigurationAccessor()
        {
            Configuration = AppConfigurations.Get(Directory.GetCurrentDirectory());
        }
    }
}
