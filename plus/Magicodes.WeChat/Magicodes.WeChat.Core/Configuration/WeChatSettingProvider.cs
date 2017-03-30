using System.Collections.Generic;
using System.Configuration;
using Abp.Configuration;
using Abp.Json;
using Abp.Zero.Configuration;
using System;

namespace Magicodes.WeChat.Configuration
{
    /// <summary>
    /// 定义公众号相关设置
    /// </summary>
    public class WeChatSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            var defaultWeChatApiSetting = new WeChatApiSetting
            {
                AppId = "wx9ff101863db7db9c",
                AppSecret = "f97b922e9cf4c1aedb4843f63b7cf6d7",
                Token = Guid.NewGuid().ToString("N"),
                WeiXinAccount = "hnxinlai"
            };

            return new[]
                   {
                       //Host settings/Tenant settings
                        new SettingDefinition(WeChatSettings.TenantManagement.WeChatApiSettings, defaultWeChatApiSetting.ToJsonString(),scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: false),
                   };
        }
    }
}
