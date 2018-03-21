using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Magicodes.App.Application.Account.Dto;

namespace Magicodes.App.Application.Account
{
    /// <summary>
    /// 账户相关
    /// </summary>
    public interface IAccountAppService : IApplicationService    
    {
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<AppRegisterOutput> AppRegister(AppRegisterInput input);

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<AppLoginOutput> AppLogin(AppLoginInput input);

        /// <summary>
        /// 授权访问
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<AppTokenAuthOutput> AppTokenAuth(AppTokenAuthInput input);

    }
}