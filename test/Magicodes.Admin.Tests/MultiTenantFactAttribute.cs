using System;
using Magicodes.Admin.Configuration;
using Xunit;
using Abp.Reflection.Extensions;

namespace Magicodes.Admin.Tests
{
    public sealed class MultiTenantFactAttribute : FactAttribute
    {
        public MultiTenantFactAttribute()
        {
            var config = AppConfigurations.Get(
                 typeof(AdminTestModule).Assembly.GetDirectoryPathOrNull()
             );

            var multiTenancyConfig = config["MultiTenancyEnabled"];
            if (multiTenancyConfig != null && multiTenancyConfig.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                Skip = "MultiTenancy is disabled.";
            }
        }
    }
}
