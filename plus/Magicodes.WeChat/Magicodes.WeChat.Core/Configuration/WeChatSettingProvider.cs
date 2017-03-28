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
                AppId = "wxe753e9de3a7ebfc2",
                AppSecret = "7cc4b4ef1daf99791d9a4f64b8f94648",
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
