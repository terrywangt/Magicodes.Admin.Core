using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Magicodes.Admin.Auditing.Dto;
using Magicodes.Admin.Auditing.Exporting;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Dto;
using Magicodes.Admin;
using Magicodes.WeChat.Application.User.Dto;
using System;
using Magicodes.WeChat.Core.User;
using Magicodes.WeChat.Core.Authorization;
using System.Diagnostics;
using Abp.BackgroundJobs;
using Abp.Runtime.Session;
using Magicodes.WeChat.Application.BackgroundJob;
using Magicodes.WeChat.Application.Configuration.Dto;
using Abp.Configuration;
using Magicodes.WeChat.Configuration;
using Newtonsoft.Json;

namespace Magicodes.WeChat.Application.Configuration
{
    [AbpAuthorize(WeChatPermissions.WeChatPermissions_Pages_Tenants_WeChatApiSetting)]
    public class WeChatApiSettingAppService : AdminAppServiceBase, IWeChatApiSettingAppService
    {
        public WeChatApiSettingAppService()
        {

        }


        public virtual async Task<WeChatApiSettingEditDto> GetWeChatApiSettingAsync()
        {
            var settingValue = await SettingManager.GetSettingValueAsync(WeChatSettings.TenantManagement.WeChatApiSettings);
            var appConfig = JsonConvert.DeserializeObject<WeChatApiSettingEditDto>(settingValue);
            return appConfig;
        }

        public virtual async Task<WeChatApiSettingEditDto> GetWeChatApiSettingForTenantAsync(int tenantId)
        {
            var settingValue = await SettingManager.GetSettingValueForTenantAsync(WeChatSettings.TenantManagement.WeChatApiSettings, tenantId);
            var appConfig = JsonConvert.DeserializeObject<WeChatApiSettingEditDto>(settingValue);
            return appConfig;
        }
    }
}
