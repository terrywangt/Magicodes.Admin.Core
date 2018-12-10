using Abp.Application.Services;
using Abp.Authorization;
using Abp.Configuration;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Configuration.MiniProgram;
using Magicodes.Admin.Configuration.MiniProgram.Dto;
using System.Threading.Tasks;

namespace Magicodes.Admin.Configuration.MiniProgram
{
    [AbpAuthorize(AppPermissions.Pages_Administration_MiniProgram_Settings)]
    public class MiniProgramSettingsAppService : ApplicationService, IMiniProgramSettingsAppService
    {
        public MiniProgramSettingsAppService(
            ISettingDefinitionManager settingDefinitionManager
        ) : base()
        {

        }

        public async Task<MiniProgramSettingsEditDto> GetAllSettings() => new MiniProgramSettingsEditDto
        {
            WeChatMiniProgram = await GetWeChatMiniProgramSettingsAsync()
        };

        private async Task<WeChatMiniProgramSettingsEditDto> GetWeChatMiniProgramSettingsAsync() => new WeChatMiniProgramSettingsEditDto
        {
            AppId = await SettingManager.GetSettingValueAsync(AppSettings.WeChatMiniProgram.AppId),
            AppSecret =
                  await SettingManager.GetSettingValueAsync(AppSettings.WeChatMiniProgram.AppSecret)
        };

        public async Task UpdateAllSettings(MiniProgramSettingsEditDto input)
        {
            await UpdateWeChatMiniProgramAsync(input.WeChatMiniProgram);
        }

        private async Task UpdateWeChatMiniProgramAsync(WeChatMiniProgramSettingsEditDto input)
        {
            await SaveSettings(AppSettings.WeChatMiniProgram.AppId, input.AppId);
            await SaveSettings(AppSettings.WeChatMiniProgram.AppSecret, input.AppSecret);
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
