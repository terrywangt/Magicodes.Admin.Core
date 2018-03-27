using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Configuration;

namespace Magicodes.Admin.Timing
{
    public interface ITimeZoneService
    {
        Task<string> GetDefaultTimezoneAsync(SettingScopes scope, int? tenantId);

        TimeZoneInfo FindTimeZoneById(string timezoneId);
        
        List<NameValueDto> GetWindowsTimezones();
    }
}
