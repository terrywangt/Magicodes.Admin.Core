using Abp.Authorization;
using Magicodes.Admin.Authorization.Roles;
using Magicodes.Admin.Authorization.Users;

namespace Magicodes.Admin.Authorization
{
    /// <summary>
    /// Implements <see cref="PermissionChecker"/>.
    /// </summary>
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
