// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : IUsersAppService.cs
//           description :
//   
//           created by 雪雁 at  2018-07-30 10:04
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System.Threading.Tasks;
using Abp.Application.Services;
using Magicodes.App.Application.Users.Dto;

namespace Magicodes.App.Application.Users
{
    /// <summary>
    ///     用户
    /// </summary>
    public interface IUsersAppService : IApplicationService
    {
        /// <summary>
        ///     注册或登陆
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<AppRegisterOrLoginOutput> AppRegisterOrLogin(AppRegisterOrLoginInput input);

        /// <summary>
        ///     授权访问
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<AppTokenAuthOutput> AppTokenAuth(AppTokenAuthInput input);

        /// <summary>
        ///     获取密码重置Code
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task SendPasswordResetCode(SendPasswordResetCodeInput input);

        /// <summary>
        ///     重置密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ResetPasswordOutput> ResetPassword(ResetPasswordInput input);
    }
}