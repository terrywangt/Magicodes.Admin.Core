using Magicodes.Admin.Configuration.Pay.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.Admin.Configuration.Pay
{
    public interface IPaySettingsAppService
    {
        Task<PaySettingEditDto> GetAllSettings();

        Task UpdateAllSettings(PaySettingEditDto input);
    }
}
