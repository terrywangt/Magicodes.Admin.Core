using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.MultiTenancy;
using Shouldly;
using Xunit;

namespace Magicodes.Admin.Tests.Authorization.Users
{
    public class UserAppService_Unlock_Tests : UserAppServiceTestBase
    {
        private readonly UserManager _userManager;
        private readonly LogInManager _loginManager;

        public UserAppService_Unlock_Tests()
        {
            _userManager = Resolve<UserManager>();
            _loginManager = Resolve<LogInManager>();

            CreateTestUsers();
        }

        [Fact]
        public async Task Should_Unlock_User()
        {
            //Arrange

            await _userManager.InitializeOptionsAsync(AbpSession.TenantId);
            var user = await GetUserByUserNameAsync("jnash");

            //Pre conditions
            (await _userManager.IsLockedOutAsync(user)).ShouldBeFalse();
            user.IsLockoutEnabled.ShouldBeTrue();

            //Try wrong password until lockout
            AbpLoginResultType loginResultType;
            do
            {
                loginResultType = (await _loginManager.LoginAsync(user.UserName, "wrong-password", Tenant.DefaultTenantName)).Result;
            } while (loginResultType != AbpLoginResultType.LockedOut);

            (await _userManager.IsLockedOutAsync(await GetUserByUserNameAsync("jnash"))).ShouldBeTrue();

            //Act

            await UserAppService.UnlockUser(new EntityDto<long>(user.Id));

            //Assert

            (await _loginManager.LoginAsync(user.UserName, "wrong-password", Tenant.DefaultTenantName)).Result.ShouldBe(AbpLoginResultType.InvalidPassword);
        }
    }
}