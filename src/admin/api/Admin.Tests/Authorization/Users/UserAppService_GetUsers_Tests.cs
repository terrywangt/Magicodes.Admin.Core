using System.Threading.Tasks;
using Abp.Authorization.Users;
using Magicodes.Admin.Authorization.Users.Dto;
using Shouldly;
using Xunit;

namespace Magicodes.Admin.Tests.Authorization.Users
{
    public class UserAppService_GetUsers_Tests : UserAppServiceTestBase
    {
        [Fact]
        public async Task Should_Get_Initial_Users()
        {
            //Act
            var output = await UserAppService.GetUsers(new GetUsersInput());

            //Assert
            output.TotalCount.ShouldBe(1);
            output.Items.Count.ShouldBe(1);
            output.Items[0].UserName.ShouldBe(AbpUserBase.AdminUserName);
        }

        [Fact]
        public async Task Should_Get_Users_Paged_And_Sorted_And_Filtered()
        {
            //Arrange
            CreateTestUsers();

            //Act
            var output = await UserAppService.GetUsers(
                new GetUsersInput
                {
                    MaxResultCount = 2,
                    Sorting = "Username"
                });

            //Assert
            output.TotalCount.ShouldBe(4);
            output.Items.Count.ShouldBe(2);
            output.Items[0].UserName.ShouldBe("adams_d");
            output.Items[1].UserName.ShouldBe(AbpUserBase.AdminUserName);
        }

        [Fact]
        public async Task Should_Get_Users_Filtered()
        {
            //Arrange
            CreateTestUsers();

            //Act
            var output = await UserAppService.GetUsers(
                new GetUsersInput
                {
                    Filter = "Adam"
                });

            //Assert
            output.TotalCount.ShouldBe(1);
            output.Items.Count.ShouldBe(1);
            output.Items[0].UserName.ShouldBe("adams_d");

            //Act
            var output2 = await UserAppService.GetUsers(
                new GetUsersInput
                {
                    Permission = "test.permission1"
                });

            //Assert
            output2.TotalCount.ShouldBe(3);
            output2.Items.Count.ShouldBe(3);
        }
    }
}
