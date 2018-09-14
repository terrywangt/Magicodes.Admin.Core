// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : SmSCodeAppService.cs
//           description :
//   
//           created by 雪雁 at  2018-07-30 10:43
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using Abp.Timing;
using Abp.UI;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Identity;
using Magicodes.App.Application.SmSCode.Dto;
using Microsoft.AspNetCore.Mvc;

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
        private readonly ICacheManager _cacheManager;
        private readonly ISmsVerificationCodeManager _smsVerificationCodeManager;
        #endregion

        #region 构造函数注入
        /// <inheritdoc />
        public SmSCodeAppService(
            ICacheManager cacheManager, ISmsVerificationCodeManager smsVerificationCodeManager)
        {
            _cacheManager = cacheManager;
            _smsVerificationCodeManager = smsVerificationCodeManager;
        }
        #endregion

        /// <summary>
        ///     请求发送短信验证码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        [HttpPost]
        [UnitOfWork(false)]
        public async Task CreateSmsCode(CreateSmsCodeInput input)
        {
            //---------------请结合以下内容编写实现（勿删）---------------
            // 验证码长度为4，60s内不得重复发送。
            // 验证码10分钟内有效。
            //------------------------------------------------------

            await _smsVerificationCodeManager.CreateAndSendVerificationMessage(input.PhoneNumber,
                input.SmsCodeType.ToString(), 60, Clock.Now.AddMinutes(10));
        }

        /// <summary>
        ///     短信验证码校验
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        [HttpPut]
        public async Task VerifySmsCode(VerifySmsCodeInputDto input)
        {
            await _smsVerificationCodeManager.VerifyCodeAndShowUserFriendlyException(input.PhoneNumber, input.Code,
                input.SmsCodeType.ToString());

            var user = GetUserByChecking(input.PhoneNumber);
            user.IsPhoneNumberConfirmed = true;
            await UserManager.UpdateAsync(user);
        }

        /// <summary>
        ///     检查用户
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        private User GetUserByChecking(string phoneNumber)
        {
            var user = UserManager.Users.FirstOrDefault(p => p.PhoneNumber == phoneNumber);
            if (user == null) throw new UserFriendlyException(L("InvalidPhoneNumber"));

            if (!user.IsActive) throw new UserFriendlyException(L("UserIsNotActive"));

            return user;
        }
    }
}