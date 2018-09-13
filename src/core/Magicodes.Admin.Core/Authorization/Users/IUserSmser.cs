using System.Threading.Tasks;

namespace Magicodes.Admin.Authorization.Users
{
    public interface IUserSmser
    {
        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        Task SendVerificationMessage(User user, string code);

        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="phoneNumber">手机号码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        Task SendVerificationMessage(string phoneNumber, string code);
    }
}
