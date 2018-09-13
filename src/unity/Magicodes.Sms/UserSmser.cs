using Magicodes.Admin.Authorization.Users;
using Magicodes.Sms.Services;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Localization;
using Abp.UI;
using Castle.Core.Logging;
using Magicodes.Admin.Localization;

namespace Magicodes.Sms
{
    public class UserSmser : IUserSmser, ITransientDependency
    {
        public ILogger Logger { get; set; }
        private readonly ISmsAppService _smsAppService;
        private readonly IAppLocalizationManager _appLocalizationManager;
        public UserSmser(ISmsAppService smsAppService, IAppLocalizationManager appLocalizationManager)
        {
            _smsAppService = smsAppService;
            _appLocalizationManager = appLocalizationManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        public Task SendVerificationMessage(User user, string code) => SendVerificationMessage(user.PhoneNumber, code);

        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="phoneNumber">手机号码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        public async Task SendVerificationMessage(string phoneNumber, string code)
        {
            var result = await _smsAppService.SmsService.SendCodeAsync(phoneNumber, code);
            if (!result.Success)
            {
                Logger.Error("短信发送失败：" + result.ErrorMessage);
                throw new UserFriendlyException(_appLocalizationManager.L("SmsSendError"));
            }
        }
    }
}
