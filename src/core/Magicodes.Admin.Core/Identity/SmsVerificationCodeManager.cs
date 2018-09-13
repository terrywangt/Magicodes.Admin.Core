using Abp;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Timing;
using Abp.UI;
using Magicodes.Admin.Identity.Cache;
using Magicodes.Admin.Localization;
using System;
using System.Threading.Tasks;

namespace Magicodes.Admin.Identity
{
    /// <summary>
    /// 短信验证码管理器
    /// </summary>
    public class SmsVerificationCodeManager : ISmsVerificationCodeManager, ITransientDependency
    {
        private readonly ISmsSender _smsSender;
        private readonly IAppLocalizationManager _appLocalizationManager;
        private readonly ICacheManager _cacheManager;

        public SmsVerificationCodeManager(ISmsSender smsSender, ICacheManager cacheManager, IAppLocalizationManager appLocalizationManager)
        {
            _smsSender = smsSender;
            _cacheManager = cacheManager;
            _appLocalizationManager = appLocalizationManager;
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="phoneNumber">手机号码</param>
        /// <param name="code">验证码</param>
        /// <param name="tag">业务标记</param>
        /// <returns></returns>
        public virtual async Task<bool> VerifyCode(string phoneNumber, string code, string tag = null)
        {
            var cacheKey = $"{phoneNumber}_{tag}";
            var cash = await _cacheManager.GetSmsVerificationCodeCache().GetOrDefaultAsync(cacheKey);
            if (cash != null && code == cash.Code)
            {
                return !cash.ExpiredTime.HasValue || Clock.Now <= cash.ExpiredTime.Value;
            }
            return false;
        }

        /// <summary>
        /// 创建验证码
        /// </summary>
        /// <param name="phoneNumber">手机号码</param>
        /// <param name="tag">业务标记</param>
        /// <param name="repeatSecs">多少秒内不得重复发送</param>
        /// <param name="expiredTime">过期时间</param>
        /// <returns></returns>
        public virtual async Task<string> Create(string phoneNumber, string tag = null, int repeatSecs = 60, DateTime? expiredTime = null)
        {
            var code = RandomHelper.GetRandom(1000, 9999).ToString();
            var cacheKey = $"{phoneNumber}_{tag}";
            var outTime = DateTime.Now.AddSeconds(-repeatSecs);
            var cash = await _cacheManager.GetSmsVerificationCodeCache().GetOrDefaultAsync(cacheKey);

            //验证码长度为4，60s内不得重复发送
            if (cash != null && cash.CreationTime >= outTime)
            {
                throw new UserFriendlyException(_appLocalizationManager.L("SmsRepeatSendTip"));
            }

            var cacheItem = new SmsVerificationCodeCacheItem { Code = code, ExpiredTime = expiredTime };
            _cacheManager.GetSmsVerificationCodeCache().Set(
                cacheKey,
                cacheItem
            );
            return code;
        }

        /// <summary>
        /// 创建并发送短信验证码
        /// </summary>
        /// <param name="phoneNumber">手机号码</param>
        /// <param name="tag">业务标记</param>
        /// <param name="repeatSecs">多少秒内不得重复发送</param>
        /// <param name="expiredTime">过期时间</param>
        /// <returns></returns>
        public virtual async Task CreateAndSendVerificationMessage(string phoneNumber, string tag = null, int repeatSecs = 60,
            DateTime? expiredTime = null)
        {
            var code = await Create(phoneNumber, tag, repeatSecs, expiredTime);
            await _smsSender.SendCodeAsync(phoneNumber, code);
        }

        /// <summary>
        /// 验证验证码并且提示友好错误信息
        /// </summary>
        /// <param name="phoneNumber">手机号码</param>
        /// <param name="code">验证码</param>
        /// <param name="tag">业务标记</param>
        /// <returns></returns>
        public virtual async Task VerifyCodeAndShowUserFriendlyException(string phoneNumber, string code, string tag = null)
        {
            var codeIsValid = await VerifyCode(phoneNumber, code, tag);
            if (!codeIsValid)
            {
                throw new UserFriendlyException(_appLocalizationManager.L("WrongSmsVerificationCode"));
            }
        }
    }
}
