using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.Admin.Sms
{
    /// <summary>
    /// 短信发送器
    /// </summary>
    public interface ISmsSender
    {
        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        Task SendAsync(string phone, string code);
    }
}
