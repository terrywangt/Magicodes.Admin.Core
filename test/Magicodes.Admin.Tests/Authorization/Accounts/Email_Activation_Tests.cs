using System.Linq;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Magicodes.Admin.Authorization.Accounts;
using Magicodes.Admin.Authorization.Accounts.Dto;
using Magicodes.Admin.Authorization.Users;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Magicodes.Admin.Tests.Authorization.Accounts
{
    public class Email_Activation_Tests : AppTestBase
    {
        [Fact]
        public async Task Should_Activate_Email()
        {
            //Arrange

            UsingDbContext(context =>
            {
                //Set IsEmailConfirmed to false to provide initial test case
                var currentUser = context.Users.Single(u => u.Id == AbpSession.UserId.Value);
                currentUser.IsEmailConfirmed = false;
            });

            var user = await GetCurrentUserAsync();
            user.IsEmailConfirmed.ShouldBeFalse();

            string confirmationCode = null;

            var fakeUserEmailer = Substitute.For<IUserEmailer>();
            fakeUserEmailer.SendEmailActivationLinkAsync(Arg.Any<User>(), Arg.Any<string>()).Returns(callInfo =>
            {
                var calledUser = callInfo.Arg<User>();
                calledUser.EmailAddress.ShouldBe(user.EmailAddress);
                confirmationCode = calledUser.EmailConfirmationCode; //Getting the confirmation code sent to the email address
                return Task.CompletedTask;
            });

            LocalIocManager.IocContainer.Register(Component.For<IUserEmailer>().Instance(fakeUserEmailer).IsDefault());

            var accountAppService = Resolve<IAccountAppService>();

            //Act

            await accountAppService.SendEmailActivationLink(
                new SendEmailActivationLinkInput
                {
                    EmailAddress = user.EmailAddress
                }
            );

            await accountAppService.ActivateEmail(
                new ActivateEmailInput
                {
                    UserId = user.Id,
                    ConfirmationCode = confirmationCode
                }
            );

            //Assert

            user = await GetCurrentUserAsync();
            user.IsEmailConfirmed.ShouldBeTrue();
        }
    }
}