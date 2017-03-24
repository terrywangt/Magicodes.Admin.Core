using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Dto;
using Magicodes.WeChat.Application.User.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeChat.Application.User
{
    public interface IWeChatUserAppService : IApplicationService
    {
        Task<PagedResultDto<WeChatUserListDto>> GetWeChatUsers(GetWeChatUsersInput input);

        Task<FileDto> GetWeChatUsersToExcel(GetWeChatUsersInput input);
    }
}
