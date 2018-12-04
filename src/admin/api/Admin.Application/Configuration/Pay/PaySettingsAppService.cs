using Abp.Application.Services;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Extensions;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Configuration.Pay.Dto;
using Magicodes.Pay.Startup;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magicodes.Admin.Configuration.Pay
{

    [AbpAuthorize(AppPermissions.Pages_Administration_Pay_Settings)]
    public class PaySettingsAppService : ApplicationService, IPaySettingsAppService
    {
        private readonly IAppConfigurationAccessor _appConfigurationAccessor;
        private readonly IIocManager _iocManager;

        public PaySettingsAppService(
            ISettingDefinitionManager settingDefinitionManager,
            IAppConfigurationAccessor appConfigurationAccessor, IIocManager iocManager) : base()
        {
            _appConfigurationAccessor = appConfigurationAccessor;
            _iocManager = iocManager;
        }

        public async Task<PaySettingEditDto> GetAllSettings() => new PaySettingEditDto
        {
            WeChatPay = await GetWeChatSettingsAsync(),
            AliPay = await GetAliPaySettingsAsync(),
            GlobalAliPay = await GetGlobalAliPaySettingsAsync()
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

        /// <summary>
        /// 获取国际支付宝设置
        /// </summary>
        /// <returns></returns>
        private async Task<GlobalAlipaySettingEditDto> GetGlobalAliPaySettingsAsync()
        {
            var dto = new GlobalAlipaySettingEditDto
            {
                Key = await SettingManager.GetSettingValueAsync(AppSettings.GlobalAliPayManagement.Key),
                Partner = await SettingManager.GetSettingValueAsync(AppSettings.GlobalAliPayManagement.Partner),
                Gatewayurl = await SettingManager.GetSettingValueAsync(AppSettings.GlobalAliPayManagement.Gatewayurl),
                Notify = await SettingManager.GetSettingValueAsync(AppSettings.GlobalAliPayManagement.Notify),
                ReturnUrl = await SettingManager.GetSettingValueAsync(AppSettings.GlobalAliPayManagement.ReturnUrl),
                Currency = await SettingManager.GetSettingValueAsync(AppSettings.GlobalAliPayManagement.Currency),
                IsActive = Convert.ToBoolean(
                    await SettingManager.GetSettingValueAsync(AppSettings.GlobalAliPayManagement.IsActive))
            };
            var splitFundSettingsString =
                await SettingManager.GetSettingValueAsync(AppSettings.GlobalAliPayManagement.SplitFundSettings);
            if (!splitFundSettingsString.IsNullOrWhiteSpace())
            {
                dto.SplitFundSettings = JsonConvert.DeserializeObject<List<SplitFundSettingDto>>(splitFundSettingsString);
            }
            return dto;
        }


        public async Task UpdateAllSettings(PaySettingEditDto input)
        {
            await UpdateWeChatSettingsAsync(input.WeChatPay);
            await UpdateAliSettingsAsync(input.AliPay);
            await UpdateGlobalAliSettingsAsync(input.GlobalAliPay);

            //配置支付
            await PayStartup.ConfigAsync(Logger, _iocManager, _appConfigurationAccessor.Configuration, SettingManager);
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

        private async Task UpdateGlobalAliSettingsAsync(GlobalAlipaySettingEditDto input)
        {
            await SaveSettings(AppSettings.GlobalAliPayManagement.Key, input.Key);
            await SaveSettings(AppSettings.GlobalAliPayManagement.Partner, input.Partner);
            await SaveSettings(AppSettings.GlobalAliPayManagement.Gatewayurl, input.Gatewayurl);
            await SaveSettings(AppSettings.GlobalAliPayManagement.Notify, input.Notify);
            await SaveSettings(AppSettings.GlobalAliPayManagement.ReturnUrl, input.ReturnUrl);
            await SaveSettings(AppSettings.GlobalAliPayManagement.Currency, input.Currency);
            await SaveSettings(AppSettings.GlobalAliPayManagement.IsActive, Convert.ToString(input.IsActive));

            if (input.SplitFundSettings != null && input.SplitFundSettings.Count > 0)
            {
                await SaveSettings(AppSettings.GlobalAliPayManagement.SplitFundSettings, JsonConvert.SerializeObject(input.SplitFundSettings));
            }
        }
    }
}
