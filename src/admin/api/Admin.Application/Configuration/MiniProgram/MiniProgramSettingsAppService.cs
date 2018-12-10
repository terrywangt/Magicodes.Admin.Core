using Abp.Application.Services;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Dependency;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Configuration.MiniProgram.Dto;
using Magicodes.MiniProgram.Startup;
using System.Threading.Tasks;

namespace Magicodes.Admin.Configuration.MiniProgram
{
    [AbpAuthorize(AppPermissions.Pages_Administration_MiniProgram_Settings)]
    public class MiniProgramSettingsAppService : ApplicationService, IMiniProgramSettingsAppService
    {
        private readonly IIocManager _iocManager;
        private readonly IAppConfigurationAccessor _appConfigurationAccessor;
        public MiniProgramSettingsAppService(
            ISettingDefinitionManager settingDefinitionManager, IIocManager iocManager, IAppConfigurationAccessor appConfigurationAccessor) : base()
        {
            _iocManager = iocManager;
            _appConfigurationAccessor = appConfigurationAccessor;
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
            //配置支付
            MiniProgramStartup.Config(Logger, _iocManager, _appConfigurationAccessor.Configuration, SettingManager);
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
