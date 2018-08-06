using Castle.Core.Logging;
using Magicodes.Admin.Configuration;
using Magicodes.Sms.Core;

namespace Magicodes.Sms.Services
{
    /// <summary>
    /// 短信发送服务
    /// </summary>
    public interface ISmsAppService
    {
        /// <summary>
        /// 短信服务
        /// </summary>
        ISmsService SmsService { get; set; }
    }
}