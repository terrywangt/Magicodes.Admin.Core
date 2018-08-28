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
        public PaySettingsAppService(
            ISettingDefinitionManager settingDefinitionManager) : base()
        {

        }

        public async Task<PaySettingEditDto> GetAllSettings()
        {
            return new PaySettingEditDto
            {
                WeChatPay = await GetWeChatSettingsAsync(),
                AliPay = await GetAliPaySettingsAsync()
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
        private async Task<AliPaySettingEditDto> GetAliPaySettingsAsync()
        {
            return new AliPaySettingEditDto
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
        }


        public async Task UpdateAllSettings(PaySettingEditDto input)
        {
            await UpdateWeChatSettingsAsync(input.WeChatPay);
            await UpdateAliSettingsAsync(input.AliPay);
        }

        private async Task UpdateWeChatSettingsAsync(WeChatPaySettingEditDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.WeChatPayManagement.AppId, input.AppId);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.WeChatPayManagement.MchId, input.MchId);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.WeChatPayManagement.TenPayKey, input.TenPayKey);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.WeChatPayManagement.PayNotifyUrl, input.PayNotifyUrl);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.WeChatPayManagement.IsActive, Convert.ToString(input.IsActive));
        }

        private async Task UpdateAliSettingsAsync(AliPaySettingEditDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.AliPayManagement.AppId, input.AppId);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.AliPayManagement.Uid, input.Uid);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.AliPayManagement.Gatewayurl, input.Gatewayurl);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.AliPayManagement.AlipayPublicKey, input.AlipayPublicKey);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.AliPayManagement.AlipaySignPublicKey, input.AlipaySignPublicKey);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.AliPayManagement.PrivateKey, input.PrivateKey);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.AliPayManagement.CharSet, input.CharSet);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.AliPayManagement.Notify, input.Notify);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.AliPayManagement.SignType, input.SignType);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.AliPayManagement.IsKeyFromFile, Convert.ToString(input.IsKeyFromFile));
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.AliPayManagement.IsActive, Convert.ToString(input.IsActive));
        }
    }
}
