using Abp.Runtime.Caching;
using Magicodes.Admin.Identity;
using Magicodes.App.Application.Users;
using Magicodes.App.Application.Users.Dto;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace App.Tests.Users
{
    public class Users_Tests : AppTestBase
    {
        private readonly IUsersAppService _usersAppService;
        private readonly ICacheManager _cacheManager;
        private readonly ISmsVerificationCodeManager _smsVerificationCodeManager;

        public Users_Tests()
        {
            _usersAppService = Resolve<IUsersAppService>();
            _cacheManager = Resolve<ICacheManager>();
            _smsVerificationCodeManager = Resolve<ISmsVerificationCodeManager>();
        }

        [Theory(DisplayName = "注册")]
        [InlineData("1367197435x", "9957fc0fa630436e821b9e087b76aaf4", (AppRegisterOrLoginInput.FromEnum)0, "0f36dbb147c7478ebab94cd68a33202e", "Test")]
        public async Task AppRegister_Test(string phone, string openId, AppRegisterOrLoginInput.FromEnum from, string unionId, string trueName)
        {
            var smsCode = await _smsVerificationCodeManager.Create(phone, "RegisterOrLogin");

            await _smsVerificationCodeManager.VerifyCodeAndShowUserFriendlyException(phone, smsCode, "RegisterOrLogin");

            var userInput = new AppRegisterOrLoginInput()
            {
                Phone = phone,
                Code = smsCode,
                OpenId = openId,
                From = from,
                UnionId = unionId,
                TrueName = trueName,
            };
            var output = await _usersAppService.AppRegisterOrLogin(userInput);
            output.ShouldNotBeNull();
        }

        [Theory(DisplayName = "登陆")]
        [InlineData("13671974358", "9957fc0fa630436e821b9e087b76aaf4", null, null)]
        [InlineData("13671974358", "9957fc0fa630436e821b9e087b76aaf4", (AppRegisterOrLoginInput.FromEnum)0, "0f36dbb147c7478ebab94cd68a33202e")]
        public async Task AppLogin_Test(string phone, string openId, AppRegisterOrLoginInput.FromEnum? from, string unionId)
        {
            var smsCode = await _smsVerificationCodeManager.Create(phone, "RegisterOrLogin");

            await _smsVerificationCodeManager.VerifyCodeAndShowUserFriendlyException(phone, smsCode, "RegisterOrLogin");

            var userInput = new AppRegisterOrLoginInput()
            {
                Phone = phone,
                Code = smsCode,
                OpenId = openId,
                From = from,
                UnionId = unionId,
            };
            //验证登陆逻辑
            var loginOutput = await _usersAppService.AppRegisterOrLogin(userInput);
            loginOutput.ShouldNotBeNull();
        }

        [Theory(DisplayName = "授权访问")]
        [InlineData("e8083f0bb73f4c2dbf687fce2d1c8009", (Magicodes.App.Application.Users.Dto.AppTokenAuthInput.FromEnum)0)]
        [InlineData("e8083f0bb73f4c2dbf687fce2d1c8010", (Magicodes.App.Application.Users.Dto.AppTokenAuthInput.FromEnum)0)]
        public async Task AppTokenAuth_Test(string openIdOrUnionId, Magicodes.App.Application.Users.Dto.AppTokenAuthInput.FromEnum from)
        {
            var input = new AppTokenAuthInput()
            {
                OpenIdOrUnionId = openIdOrUnionId,
                From = from,
            };
            var output = await _usersAppService.AppTokenAuth(input);
            output.ShouldNotBeNull();
        }

        [Theory(DisplayName = "重置密码")]
        [InlineData("13671974358")]
        public async Task SendPasswordResetCode_Test(string phone)
        {
            var input = new SendPasswordResetCodeInput()
            {
                PhoneNumber = phone
            };

            await _usersAppService.SendPasswordResetCode(input);

            var user = UsingDbContext(context => context.Users.FirstOrDefault(p => p.PhoneNumber == phone));

            var reseInput = new ResetPasswordInput()
            {
                UserId = user.Id,
                ResetCode = user.PasswordResetCode,
                Password = "123456"
            };
            var reseUser = await _usersAppService.ResetPassword(reseInput);
            reseUser.ShouldNotBeNull();
        }
    }
}