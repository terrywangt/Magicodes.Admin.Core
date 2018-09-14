using System.Linq;
using Abp.Runtime.Caching;
using Magicodes.Admin.Identity;
using Magicodes.App.Application.Users;
using Magicodes.App.Application.Users.Dto;
using Shouldly;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Magicodes.Admin.Authorization.Users;
using Xunit;

namespace App.Tests.Users
{
    public class Users_Tests : AppTestBase
    {
        private readonly IUsersAppService _usersAppService;
        private readonly ICacheManager _cacheManager;
        private readonly ISmsVerificationCodeManager _smsVerificationCodeManager;
        private readonly UserManager _userManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public Users_Tests()
        {
            _usersAppService = Resolve<IUsersAppService>();
            _cacheManager = Resolve<ICacheManager>();
            _smsVerificationCodeManager = Resolve<ISmsVerificationCodeManager>();
            _userManager = Resolve<UserManager>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Theory(DisplayName = "登陆", Skip = "暂不实现APP登陆")]
        [InlineData("Test", "Test")]
        public async Task AppLogin_Test(string phone, string code)
        {
            var input = new AppLoginInput()
            {
                Phone = phone,
                Code = code,
            };
            var output = await _usersAppService.AppLogin(input);
            output.ShouldNotBeNull();
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

        [Theory(DisplayName = "发送验证码,注册,重置密码")]
        [InlineData("18975060440", "1043", "9957fc0fa630436e821b9e087b76aaf4", (AppRegisterInput.FromEnum)0, "0f36dbb147c7478ebab94cd68a33202e", "Test")]
        [UnitOfWork(IsDisabled = true)]
        public async Task SendPasswordResetCode_Test(string phone, string code, string openId, AppRegisterInput.FromEnum from, string unionId, string trueName)
        {
            var smsCode = await _smsVerificationCodeManager.Create(phone, "Register");

            await _smsVerificationCodeManager.VerifyCodeAndShowUserFriendlyException(phone, smsCode, "Register");

            var userInput = new AppRegisterInput()
            {
                Phone = phone,
                Code = smsCode,
                OpenId = openId,
                From = from,
                UnionId = unionId,
                TrueName = trueName,
            };
            var output = await _usersAppService.AppRegister(userInput);
            output.ShouldNotBeNull();

            var input = new SendPasswordResetCodeInput()
            {
                PhoneNumber = phone
            };

            await _usersAppService.SendPasswordResetCode(input);

            User user;
            using (var unitOfWork =
                _unitOfWorkManager.Begin(new UnitOfWorkOptions()))
            {
                user = _userManager.Users.FirstOrDefault(p => p.PhoneNumber == phone);
                unitOfWork.Complete();
            }


            using (var unitOfWork =
                _unitOfWorkManager.Begin(new UnitOfWorkOptions()))
            {
                var reseInput = new ResetPasswordInput()
                {
                    UserId = user.Id,
                    ResetCode = user.PasswordResetCode,
                    Password = "123456"
                };

                var reseUser = await _usersAppService.ResetPassword(reseInput);
                reseUser.ShouldNotBeNull();
                unitOfWork.Complete();
            }
        }
    }
}