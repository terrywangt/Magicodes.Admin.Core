using System.Threading.Tasks;
using Abp.Runtime.Validation;
using Abp.UI;
using Magicodes.App.Application.SmSCode;
using Magicodes.App.Application.SmSCode.Dto;
using Xunit;

namespace App.Tests.SmSCode
{
    public class SmSCode_Tests : AppTestBase
    {
        private readonly ISmSCodeAppService smSCodeAppService;

        public SmSCode_Tests()
        {
            smSCodeAppService = Resolve<ISmSCodeAppService>();
        }

        [Theory(DisplayName = "请求发送短信验证码")]
        //[InlineData("", (CreateSmsCodeInput.SmsCodeTypeEnum)0)]
        [InlineData("13671974358", (CreateSmsCodeInput.SmsCodeTypeEnum)0)]
        public async Task CreateSmsCode_Test(string phone, CreateSmsCodeInput.SmsCodeTypeEnum smsCodeType)
        {
            //---------------请结合以下要点编写单元测试（勿删）---------------
            // 验证码长度为4，60s内不得重复发送
            //---------------------------------------------------------
            var input = new CreateSmsCodeInput()
            {
                PhoneNumber = phone,
                SmsCodeType = smsCodeType,
            };
            if (string.IsNullOrWhiteSpace(phone))
            {
                await Assert.ThrowsAsync<AbpValidationException>(async () => await smSCodeAppService.CreateSmsCode(input: input));
            }
            else
            {
                
                await smSCodeAppService.CreateSmsCode(input);

                //重复验证
                await Assert.ThrowsAsync<UserFriendlyException>(async () => await smSCodeAppService.CreateSmsCode(input: input));
            }
        }
    }
}