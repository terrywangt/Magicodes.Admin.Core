using System.Threading.Tasks;

namespace Magicodes.Admin.Identity
{
    /// <summary>
    /// 短信发送服务
    /// </summary>
    public interface ISmsSender
    {
        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        Task SendCodeAsync(string phone, string code);
    }
}