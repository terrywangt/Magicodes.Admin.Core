using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.Admin.Sms
{
    public class NullSmsSender : ISmsSender, ITransientDependency
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static NullSmsSender Instance { get; } = new NullSmsSender();

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task SendAsync(string phone, string code)
        {
            return Task.FromResult(0);
        }
    }
}
