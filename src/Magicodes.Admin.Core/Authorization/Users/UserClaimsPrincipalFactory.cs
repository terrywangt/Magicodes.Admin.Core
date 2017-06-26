using System.Security.Claims;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Uow;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Magicodes.Admin.Authorization.Roles;

namespace Magicodes.Admin.Authorization.Users
{
    public class UserClaimsPrincipalFactory : AbpUserClaimsPrincipalFactory<User, Role>
    {
        public UserClaimsPrincipalFactory(
            UserManager userManager,
            RoleManager roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(
                  userManager,
                  roleManager,
                  optionsAccessor)
        {
        }
    }
}
