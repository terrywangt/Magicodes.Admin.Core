using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Magicodes.Admin.Core.Custom.LogInfos;
using Magicodes.App.Application.SmSCode.Dto;
using Magicodes.Sms.Aliyun;
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

        #endregion

        #region 构造函数注入

        public SmSCodeAppService(
            IRepository<SmsCodeLog, long> smsCodeLogRepository
        )
        {
            _smsCodeLogRepository = smsCodeLogRepository;
        }

        #endregion

        /// <summary>
        ///     请求发送短信验证码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        [HttpPost]
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
            //暂时写死使用阿里云
            var smsService = new AliyunSmsService();
            var result = await smsService.SendCodeAsync(input.Phone, code);
            if (!result.Success) throw new UserFriendlyException(result.ErrorMessage);
            await _smsCodeLogRepository.InsertAsync(smsCodeLog);
        }
    }
}