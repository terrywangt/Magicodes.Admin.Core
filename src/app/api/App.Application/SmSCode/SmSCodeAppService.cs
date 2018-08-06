using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Magicodes.Admin.Core.Custom.LogInfos;
using Magicodes.App.Application.SmSCode.Dto;
using Magicodes.Sms.Aliyun;
using Magicodes.Sms.Services;
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

        private readonly IRepository<SmsCodeLog, long> _smsCodeLogRepository;
        private readonly ISmsAppService _smsAppService;

        #endregion

        #region 构造函数注入

        public SmSCodeAppService(
            IRepository<SmsCodeLog, long> smsCodeLogRepository, ISmsAppService smsAppService)
        {
            _smsCodeLogRepository = smsCodeLogRepository;
            _smsAppService = smsAppService;
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
            var code = new Random().Next(1000, 9999).ToString();
            var codeType = SmsCodeTypes.BindPhone;
            switch (input.SmsCodeType)
            {
                case CreateSmsCodeInput.SmsCodeTypeEnum.Register:
                    codeType = SmsCodeTypes.Register;
                    break;
                case CreateSmsCodeInput.SmsCodeTypeEnum.Login:
                    codeType = SmsCodeTypes.Login;
                    break;
                default:
                    break;
            }

            var outTime = DateTime.Now.AddSeconds(-60);
            //验证码长度为4，60s内不得重复发送
            if (_smsCodeLogRepository.GetAll().Any(p =>
                p.Phone == input.Phone && p.SmsCodeType == codeType && p.CreationTime >= outTime))
                throw new UserFriendlyException("请勿重复发送！");
            var smsCodeLog = new SmsCodeLog
            {
                Phone = input.Phone,
                SmsCode = code,
                SmsCodeType = codeType
            };
            
            var smsService = _smsAppService.SmsService;
            var result = await smsService.SendCodeAsync(input.Phone, code);
            if (!result.Success) throw new UserFriendlyException(result.ErrorMessage);
            await _smsCodeLogRepository.InsertAsync(smsCodeLog);
        }
    }
}