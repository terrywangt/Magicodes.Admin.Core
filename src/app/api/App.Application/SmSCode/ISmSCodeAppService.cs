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
        /// 短信验证码校验
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task VerifySmsCode(VerifySmsCodeInputDto input);
    }
}