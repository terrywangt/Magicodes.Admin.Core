// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : ISmSCodeAppService.cs
//           description :
//   
//           created by 雪雁 at  2018-07-30 10:43
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System.Threading.Tasks;
using Abp.Application.Services;
using Magicodes.App.Application.SmSCode.Dto;

namespace Magicodes.App.Application.SmSCode
{
    /// <summary>
    ///     短信验证码
    /// </summary>
    public interface ISmSCodeAppService : IApplicationService
    {
        /// <summary>
        ///     请求发送短信验证码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task CreateSmsCode(CreateSmsCodeInput input);

        /// <summary>
        ///     短信验证码校验
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task VerifySmsCode(VerifySmsCodeInputDto input);
    }
}