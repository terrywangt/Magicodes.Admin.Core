using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Dto;
using Magicodes.WeChat.Application.Configuration.Dto;
using Magicodes.WeChat.Application.User.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeChat.Application.Configuration
{
    public interface IWeChatApiSettingAppService : IApplicationService
    {
        Task<WeChatApiSettingEditDto> GetWeChatApiSettingAsync();
        Task<WeChatApiSettingEditDto> GetWeChatApiSettingForTenantAsync(int tenantId);
    }
}
