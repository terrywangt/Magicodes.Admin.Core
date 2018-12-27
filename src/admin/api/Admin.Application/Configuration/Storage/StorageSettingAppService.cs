using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Configuration;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Configuration.Storage.Dto;

namespace Magicodes.Admin.Configuration.Storage
{
    [AbpAuthorize(AppPermissions.Pages_Administration_Storage_Settings)]
    public class StorageSettingAppService : ApplicationService, IStorageSettingAppService
    {
        public StorageSettingAppService(ISettingDefinitionManager settingDefinitionManager) : base()
        {

        }

        public async Task<StorageSettingEditDto> GetAllSettings() => new StorageSettingEditDto
        {
            AliStorageSetting = await GetAliStorageSettingsAsync(),
            TencentStorageSetting = await GetTencentStorageSettingsAsync()
        };

        private async Task<TencentStorageSettingEditDto> GetTencentStorageSettingsAsync() => new TencentStorageSettingEditDto
        {
            IsEnabled = Convert.ToBoolean(
                await SettingManager.GetSettingValueAsync(AppSettings.TencentStorageManagement.IsEnabled)),
            AppId = await SettingManager.GetSettingValueAsync(AppSettings.TencentStorageManagement.AppId),
            BucketName = await SettingManager.GetSettingValueAsync(AppSettings.TencentStorageManagement.BucketName),
            Region = await SettingManager.GetSettingValueAsync(AppSettings.TencentStorageManagement.Region),
            SecretId = await SettingManager.GetSettingValueAsync(AppSettings.TencentStorageManagement.SecretId),
            SecretKey = await SettingManager.GetSettingValueAsync(AppSettings.TencentStorageManagement.SecretKey),
        };

        private async Task<AliStorageSettingEditDto> GetAliStorageSettingsAsync() => new AliStorageSettingEditDto
        {
            IsEnabled = Convert.ToBoolean(
                await SettingManager.GetSettingValueAsync(AppSettings.AliStorageManagement.IsEnabled)),
            AccessKeyId = await SettingManager.GetSettingValueAsync(AppSettings.AliStorageManagement.AccessKeyId),
            AccessKeySecret =
                await SettingManager.GetSettingValueAsync(AppSettings.AliStorageManagement.AccessKeySecret),
            EndPoint = await SettingManager.GetSettingValueAsync(AppSettings.AliStorageManagement.EndPoint),
            BucketName = await SettingManager.GetSettingValueAsync(AppSettings.AliStorageManagement.BucketName),
        };

        public async Task UpdateAllSettings(StorageSettingEditDto input)
        {
            await UpdateAliStorageSettingsAsync(input.AliStorageSetting);
            await UpdateTencentStorageSettingsAsync(input.TencentStorageSetting);

        }

        private async Task UpdateTencentStorageSettingsAsync(TencentStorageSettingEditDto input)
        {
            await SaveSettings(AppSettings.TencentStorageManagement.IsEnabled, input.IsEnabled.ToString());
            await SaveSettings(AppSettings.TencentStorageManagement.AppId, input.AppId);
            await SaveSettings(AppSettings.TencentStorageManagement.SecretId, input.SecretId);
            await SaveSettings(AppSettings.TencentStorageManagement.SecretKey, input.SecretKey);
            await SaveSettings(AppSettings.TencentStorageManagement.Region, input.Region);
            await SaveSettings(AppSettings.TencentStorageManagement.BucketName, input.BucketName);
        }

        private async Task UpdateAliStorageSettingsAsync(AliStorageSettingEditDto input)
        {
            await SaveSettings(AppSettings.AliStorageManagement.IsEnabled, input.IsEnabled.ToString());
            await SaveSettings(AppSettings.AliStorageManagement.AccessKeyId, input.AccessKeyId);
            await SaveSettings(AppSettings.AliStorageManagement.AccessKeySecret, input.AccessKeySecret);
            await SaveSettings(AppSettings.AliStorageManagement.EndPoint, input.EndPoint);
            await SaveSettings(AppSettings.AliStorageManagement.BucketName, input.BucketName);
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
