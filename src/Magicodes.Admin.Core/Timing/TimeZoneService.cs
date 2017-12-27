using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Timing;
using TimeZoneConverter;

namespace Magicodes.Admin.Timing
{
    public class TimeZoneService : ITimeZoneService, ITransientDependency
    {
        readonly ISettingManager _settingManager;
        readonly ISettingDefinitionManager _settingDefinitionManager;

        public TimeZoneService(
            ISettingManager settingManager,
            ISettingDefinitionManager settingDefinitionManager)
        {
            _settingManager = settingManager;
            _settingDefinitionManager = settingDefinitionManager;
        }

        public async Task<string> GetDefaultTimezoneAsync(SettingScopes scope, int? tenantId)
        {
            if (scope == SettingScopes.User)
            {
                if (tenantId.HasValue)
                {
                    return await _settingManager.GetSettingValueForTenantAsync(TimingSettingNames.TimeZone, tenantId.Value);
                }

                return await _settingManager.GetSettingValueForApplicationAsync(TimingSettingNames.TimeZone);
            }

            if (scope == SettingScopes.Tenant)
            {
                return await _settingManager.GetSettingValueForApplicationAsync(TimingSettingNames.TimeZone);
            }

            if (scope == SettingScopes.Application)
            {
                var timezoneSettingDefinition = _settingDefinitionManager.GetSettingDefinition(TimingSettingNames.TimeZone);
                return timezoneSettingDefinition.DefaultValue;
            }

            throw new Exception("Unknown scope for default timezone setting.");
        }

        public List<NameValueDto> GetWindowsTimezones()
        {
            return TZConvert.KnownWindowsTimeZoneIds.Select(TZConvert.GetTimeZoneInfo)
                .OrderBy(tz => tz.BaseUtcOffset)
                .Select(tz => new NameValueDto
                {
                    Value = tz.Id,
                    Name = tz.DisplayName
                }).ToList();
        }
    }
}