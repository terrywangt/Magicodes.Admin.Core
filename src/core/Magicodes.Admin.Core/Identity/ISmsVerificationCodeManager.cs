using System;
using System.Threading.Tasks;

namespace Magicodes.Admin.Identity
{
    /// <summary>
    /// 短信验证码管理器
    /// </summary>
    public interface ISmsVerificationCodeManager
    {
        /// <summary>
        /// 创建验证码
        /// </summary>
        /// <param name="phoneNumber">手机号码</param>
        /// <param name="tag">业务标记</param>
        /// <param name="repeatSecs">多少秒内不得重复发送</param>
        /// <param name="expiredTime">过期时间</param>
        /// <returns></returns>
        Task<string> Create(string phoneNumber, string tag = null, int repeatSecs = 60, DateTime? expiredTime = null);

        /// <summary>
        /// 创建并发送短信验证码
        /// </summary>
        /// <param name="phoneNumber">手机号码</param>
        /// <param name="tag">业务标记</param>
        /// <param name="repeatSecs">多少秒内不得重复发送</param>
        /// <param name="expiredTime">过期时间</param>
        /// <returns></returns>
        Task CreateAndSendVerificationMessage(string phoneNumber, string tag = null, int repeatSecs = 60, DateTime? expiredTime = null);

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="phoneNumber">手机号码</param>
        /// <param name="code">验证码</param>
        /// <param name="tag">业务标记</param>
        /// <returns></returns>
        Task<bool> VerifyCode(string phoneNumber, string code, string tag = null);

        /// <summary>
        /// 验证验证码并且提示友好错误信息
        /// </summary>
        /// <param name="phoneNumber">手机号码</param>
        /// <param name="code">验证码</param>
        /// <param name="tag">业务标记</param>
        /// <returns></returns>
        Task VerifyCodeAndShowUserFriendlyException(string phoneNumber, string code, string tag = null);
    }
}
