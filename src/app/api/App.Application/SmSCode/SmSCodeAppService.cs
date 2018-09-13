using Abp;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using Abp.UI;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Core.Custom.LogInfos;
using Magicodes.App.Application.SmSCode.Dto;
using Magicodes.App.Application.Users.Cache;
using Magicodes.Sms.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Magicodes.App.Application.SmSCode
{
    /// <summary>
    ///     短信验证码
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SmSCodeAppService : AppServiceBase, ISmSCodeAppService
    {
        #region 依赖注入字段

        private readonly IRepository<SmsCodeLog, long> _smsCodeLogRepository;
        private readonly ISmsAppService _smsAppService;
        private readonly IUserSmser _userSmser;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region 构造函数注入

        /// <inheritdoc />
        public SmSCodeAppService(
            IRepository<SmsCodeLog, long> smsCodeLogRepository, ISmsAppService smsAppService, IUserSmser userSmser, ICacheManager cacheManager)
        {
            _smsCodeLogRepository = smsCodeLogRepository;
            _smsAppService = smsAppService;
            _userSmser = userSmser;
            _cacheManager = cacheManager;
        }

        #endregion

        /// <summary>
        ///     请求发送短信验证码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        [HttpPost]
        [UnitOfWork(isTransactional: false)]
        public async Task CreateSmsCode(CreateSmsCodeInput input)
        {
            //---------------请结合以下内容编写实现（勿删）---------------
            // 验证码长度为4，60s内不得重复发送
            //------------------------------------------------------
            GetUserByChecking(input.PhoneNumber);

            var code = RandomHelper.GetRandom(1000, 9999).ToString();
            var cacheKey = $"{input.PhoneNumber}_{input.SmsCodeType}";
            var outTime = DateTime.Now.AddSeconds(-60);
            var cash = await _cacheManager.GetSmsVerificationCodeCache().GetOrDefaultAsync(cacheKey);

            //验证码长度为4，60s内不得重复发送
            if (cash != null && cash.CreationTime >= outTime)
            {
                throw new UserFriendlyException(L("SmsRepeatSendTip"));
            }

            var cacheItem = new SmsVerificationCodeCacheItem { Code = code };
            _cacheManager.GetSmsVerificationCodeCache().Set(
                cacheKey,
                cacheItem
            );

            await _userSmser.SendVerificationMessage(input.PhoneNumber, code);
        }

        /// <summary>
        /// 短信验证码校验
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        [HttpPut]
        public async Task VerifySmsCode(VerifySmsCodeInputDto input)
        {
            var cacheKey = $"{input.PhoneNumber}_{input.SmsCodeType}";
            var cash = await _cacheManager.GetSmsVerificationCodeCache().GetOrDefaultAsync(cacheKey);

            if (cash == null || input.Code != cash.Code)
            {
                throw new UserFriendlyException(L("WrongSmsVerificationCode"));
            }

            var user = GetUserByChecking(input.PhoneNumber);
            user.IsPhoneNumberConfirmed = true;
            await UserManager.UpdateAsync(user);
        }

        /// <summary>
        /// 检查用户
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        private User GetUserByChecking(string phoneNumber)
        {
            var user = UserManager.Users.FirstOrDefault(p => p.PhoneNumber == phoneNumber);
            if (user == null)
            {
                throw new UserFriendlyException(L("InvalidPhoneNumber"));
            }

            if (!user.IsActive)
            {
                throw new UserFriendlyException(L("UserIsNotActive"));
            }

            return user;
        }
    }
}