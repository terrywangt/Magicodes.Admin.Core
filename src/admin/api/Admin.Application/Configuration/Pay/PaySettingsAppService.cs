using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Configuration;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Configuration.Pay.Dto;

namespace Magicodes.Admin.Configuration.Pay
{

    [AbpAuthorize(AppPermissions.Pages_Administration_Host_Settings)]
    public class PaySettingsAppService : ApplicationService, IPaySettingsAppService
    {
        readonly ISettingDefinitionManager _settingDefinitionManager;

        public PaySettingsAppService(
            ISettingDefinitionManager settingDefinitionManager) : base()
        {
            _settingDefinitionManager = settingDefinitionManager;
        }

        public async Task<PaySettingEditDto> GetAllSettings()
        {
            return new PaySettingEditDto
            {
                WeChatPay = await GetWeChatSettingsAsync(),
            };
        }

        private async Task<WeChatPaySettingEditDto> GetWeChatSettingsAsync()
        {
            return new WeChatPaySettingEditDto
                        {
                            AppId = await SettingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.AppId),
                            MchId = await SettingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.MchId),
                            TenPayKey = await SettingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.TenPayKey),
                            PayNotifyUrl = await SettingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.PayNotifyUrl),
                            IsActive = Convert.ToBoolean(await SettingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.IsActive))
                        };
        }

        public async Task UpdateAllSettings(PaySettingEditDto input)
        {
            await UpdateWeChatSettingsAsync(input.WeChatPay);
        }

        private async Task UpdateWeChatSettingsAsync(WeChatPaySettingEditDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.WeChatPayManagement.AppId, input.AppId);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.WeChatPayManagement.MchId, input.MchId);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.WeChatPayManagement.TenPayKey, input.TenPayKey);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.WeChatPayManagement.PayNotifyUrl, input.PayNotifyUrl);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.WeChatPayManagement.IsActive, Convert.ToString(input.IsActive));
        }
    }
}
