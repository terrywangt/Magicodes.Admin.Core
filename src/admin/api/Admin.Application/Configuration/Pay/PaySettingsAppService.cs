using Abp.Application.Services;
using Abp.Authorization;
using Abp.Configuration;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Configuration.Pay.Dto;
using System;
using System.Threading.Tasks;

namespace Magicodes.Admin.Configuration.Pay
{

    [AbpAuthorize(AppPermissions.Pages_Administration_Pay_Settings)]
    public class PaySettingsAppService : ApplicationService, IPaySettingsAppService
    {
        public PaySettingsAppService(
            ISettingDefinitionManager settingDefinitionManager) : base()
        {

        }

        public async Task<PaySettingEditDto> GetAllSettings() => new PaySettingEditDto
        {
            WeChatPay = await GetWeChatSettingsAsync(),
            AliPay = await GetAliPaySettingsAsync()
        };

        private async Task<WeChatPaySettingEditDto> GetWeChatSettingsAsync() => new WeChatPaySettingEditDto
        {
            AppId = await SettingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.AppId),
            MchId = await SettingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.MchId),
            TenPayKey = await SettingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.TenPayKey),
            PayNotifyUrl = await SettingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.PayNotifyUrl),
            IsActive = Convert.ToBoolean(await SettingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.IsActive))
        };
        private async Task<AliPaySettingEditDto> GetAliPaySettingsAsync() => new AliPaySettingEditDto
        {
            AppId = await SettingManager.GetSettingValueAsync(AppSettings.AliPayManagement.AppId),
            Uid = await SettingManager.GetSettingValueAsync(AppSettings.AliPayManagement.Uid),
            Gatewayurl = await SettingManager.GetSettingValueAsync(AppSettings.AliPayManagement.Gatewayurl),
            AlipayPublicKey = await SettingManager.GetSettingValueAsync(AppSettings.AliPayManagement.AlipayPublicKey),
            AlipaySignPublicKey = await SettingManager.GetSettingValueAsync(AppSettings.AliPayManagement.AlipaySignPublicKey),
            PrivateKey = await SettingManager.GetSettingValueAsync(AppSettings.AliPayManagement.PrivateKey),
            CharSet = await SettingManager.GetSettingValueAsync(AppSettings.AliPayManagement.CharSet),
            Notify = await SettingManager.GetSettingValueAsync(AppSettings.AliPayManagement.Notify),
            SignType = await SettingManager.GetSettingValueAsync(AppSettings.AliPayManagement.SignType),
            IsKeyFromFile = Convert.ToBoolean(await SettingManager.GetSettingValueAsync(AppSettings.AliPayManagement.IsKeyFromFile)),
            IsActive = Convert.ToBoolean(await SettingManager.GetSettingValueAsync(AppSettings.AliPayManagement.IsActive))
        };


        public async Task UpdateAllSettings(PaySettingEditDto input)
        {
            await UpdateWeChatSettingsAsync(input.WeChatPay);
            await UpdateAliSettingsAsync(input.AliPay);
        }

        private async Task UpdateWeChatSettingsAsync(WeChatPaySettingEditDto input)
        {
            await SaveSettings(AppSettings.WeChatPayManagement.AppId, input.AppId);
            await SaveSettings(AppSettings.WeChatPayManagement.MchId, input.MchId);
            await SaveSettings(AppSettings.WeChatPayManagement.TenPayKey, input.TenPayKey);
            await SaveSettings(AppSettings.WeChatPayManagement.PayNotifyUrl, input.PayNotifyUrl);
            await SaveSettings(AppSettings.WeChatPayManagement.IsActive, Convert.ToString(input.IsActive));
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="key">设置键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        private async Task SaveSettings(string key, string value)
        {
            if (AbpSession.TenantId.HasValue)
            {
                await SettingManager.ChangeSettingForTenantAsync(AbpSession.TenantId.Value, key, value);
            }
            else
            {
                await SettingManager.ChangeSettingForApplicationAsync(key, value);
            }
        }

        private async Task UpdateAliSettingsAsync(AliPaySettingEditDto input)
        {
            await SaveSettings(AppSettings.AliPayManagement.AppId, input.AppId);
            await SaveSettings(AppSettings.AliPayManagement.Uid, input.Uid);
            await SaveSettings(AppSettings.AliPayManagement.Gatewayurl, input.Gatewayurl);
            await SaveSettings(AppSettings.AliPayManagement.AlipayPublicKey, input.AlipayPublicKey);
            await SaveSettings(AppSettings.AliPayManagement.AlipaySignPublicKey, input.AlipaySignPublicKey);
            await SaveSettings(AppSettings.AliPayManagement.PrivateKey, input.PrivateKey);
            await SaveSettings(AppSettings.AliPayManagement.CharSet, input.CharSet);
            await SaveSettings(AppSettings.AliPayManagement.Notify, input.Notify);
            await SaveSettings(AppSettings.AliPayManagement.SignType, input.SignType);
            await SaveSettings(AppSettings.AliPayManagement.IsKeyFromFile, Convert.ToString(input.IsKeyFromFile));
            await SaveSettings(AppSettings.AliPayManagement.IsActive, Convert.ToString(input.IsActive));
        }
    }
}
