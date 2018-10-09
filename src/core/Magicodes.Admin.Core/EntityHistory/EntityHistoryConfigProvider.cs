using System.Collections.Generic;
using System.Linq;
using Abp.Configuration;
using Abp.Configuration.Startup;

namespace Magicodes.Admin.EntityHistory
{
    public class EntityHistoryConfigProvider : ICustomConfigProvider
    {
        private readonly IAbpStartupConfiguration _abpStartupConfiguration;

        public EntityHistoryConfigProvider(IAbpStartupConfiguration abpStartupConfiguration)
        {
            _abpStartupConfiguration = abpStartupConfiguration;
        }

        public Dictionary<string, object> GetConfig(CustomConfigProviderContext customConfigProviderContext)
        {
            var entityHistoryConfig = new Dictionary<string, object>();

            if (!_abpStartupConfiguration.EntityHistory.IsEnabled)
            {
                return entityHistoryConfig;
            }

            foreach (var type in EntityHistoryHelper.TrackedTypes)
            {
                if (_abpStartupConfiguration.EntityHistory.Selectors.Any(s => s.Predicate(type)))
                {
                    entityHistoryConfig.Add(type.FullName ?? "", type.FullName);
                }
            }

            return entityHistoryConfig;
        }
    }
}
