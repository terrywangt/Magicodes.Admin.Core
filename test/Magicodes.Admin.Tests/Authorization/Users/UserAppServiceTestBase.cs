using System.Collections.Generic;
using Abp.Authorization.Users;
using Magicodes.Admin.Authorization.Users;

namespace Magicodes.Admin.Tests.Authorization.Users
{
    public abstract class UserAppServiceTestBase : AppTestBase
    {
        protected readonly IUserAppService UserAppService;

        protected UserAppServiceTestBase()
        {
            UserAppService = Resolve<IUserAppService>();
        }

        protected void CreateTestUsers()
        {
            //Note: There is a default "admin" user also

            UsingDbContext(
                context =>
                {
                    context.Users.Add(CreateUserEntity("jnash", "John", "Nash", "jnsh2000@testdomain.com"));
                    context.Users.Add(CreateUserEntity("adams_d", "Douglas", "Adams", "adams_d@gmail.com"));
                    context.Users.Add(CreateUserEntity("artdent", "Arthur", "Dent", "ArthurDent@yahoo.com"));
                });
        }

        protected User CreateUserEntity(string userName, string name, string surname, string emailAddress)
        {
            var user = new User
            {
                EmailAddress = emailAddress,
                IsEmailConfirmed = true,
                Name = name,
                Surname = surname,
                UserName = userName,
                Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==", //123qwe
                TenantId = AbpSession.TenantId,
                Permissions = new List<UserPermissionSetting>
                {
                    new UserPermissionSetting {Name = "test.permission1", IsGranted = true, TenantId = AbpSession.TenantId},
                    new UserPermissionSetting {Name = "test.permission2", IsGranted = true, TenantId = AbpSession.TenantId},
                    new UserPermissionSetting {Name = "test.permission3", IsGranted = false, TenantId = AbpSession.TenantId},
                    new UserPermissionSetting {Name = "test.permission4", IsGranted = false, TenantId = AbpSession.TenantId}
                }
            };

            user.SetNormalizedNames();

            return user;
        }
    }
}