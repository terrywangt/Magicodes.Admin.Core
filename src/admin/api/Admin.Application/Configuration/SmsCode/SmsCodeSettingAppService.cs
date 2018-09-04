using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Configuration;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Configuration.SmsCode.Dto;

namespace Magicodes.Admin.Configuration.SmsCode
{
    [AbpAuthorize(AppPermissions.Pages_Administration_SmsCode_Settings)]
    public class SmsCodeSettingAppService : ApplicationService, ISmsCodeSettingAppService
    {
        public SmsCodeSettingAppService(
            ISettingDefinitionManager settingDefinitionManager) : base()
        {

        }

        public async Task<SmsCodeSettingEditDto> GetAllSettings() => new SmsCodeSettingEditDto
        {
            AliSmsCodeSetting= await GetAliSmsCodeSettingsAsync()
        };

        private async Task<AliSmsCodeSettingEditDto> GetAliSmsCodeSettingsAsync() => new AliSmsCodeSettingEditDto
        {
            IsEnabled = Convert.ToBoolean(
                await SettingManager.GetSettingValueAsync(AppSettings.AliSmsCodeManagement.IsEnabled)),
            AccessKeyId = await SettingManager.GetSettingValueAsync(AppSettings.AliSmsCodeManagement.AccessKeyId),
            AccessKeySecret =
                await SettingManager.GetSettingValueAsync(AppSettings.AliSmsCodeManagement.AccessKeySecret),
            SignName = await SettingManager.GetSettingValueAsync(AppSettings.AliSmsCodeManagement.SignName),
            TemplateCode = await SettingManager.GetSettingValueAsync(AppSettings.AliSmsCodeManagement.TemplateCode),
            TemplateParam = await SettingManager.GetSettingValueAsync(AppSettings.AliSmsCodeManagement.TemplateParam)
        };

        public async Task UpdateAllSettings(SmsCodeSettingEditDto input)
        {
            await UpdateAliCmsCodeSettingsAsync(input.AliSmsCodeSetting);
        }

        private async Task UpdateAliCmsCodeSettingsAsync(AliSmsCodeSettingEditDto input)
        {
            await SaveSettings(AppSettings.AliSmsCodeManagement.IsEnabled,Convert.ToString(input.IsEnabled));
            await SaveSettings(AppSettings.AliSmsCodeManagement.AccessKeyId, input.AccessKeyId);
            await SaveSettings(AppSettings.AliSmsCodeManagement.AccessKeySecret, input.AccessKeySecret);
            await SaveSettings(AppSettings.AliSmsCodeManagement.SignName, input.SignName);
            await SaveSettings(AppSettings.AliSmsCodeManagement.TemplateCode, input.TemplateCode);
            await SaveSettings(AppSettings.AliSmsCodeManagement.TemplateParam, input.TemplateParam);
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

    }
}
