using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Magicodes.Admin.Configuration.SmsCode.Dto;

namespace Magicodes.Admin.Configuration.SmsCode
{
    public interface ISmsCodeSettingAppService
    {
        Task<SmsCodeSettingEditDto> GetAllSettings();

        Task UpdateAllSettings(SmsCodeSettingEditDto input);
    }
}
