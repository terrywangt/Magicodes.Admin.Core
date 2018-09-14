using System.Threading.Tasks;
using Abp.Dependency;

namespace Magicodes.Admin.Identity
{
    public class NullSmsSender : ISmsSender
    {
        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        public Task SendCodeAsync(string phone, string code) => Task.FromResult(0);
    }
}
