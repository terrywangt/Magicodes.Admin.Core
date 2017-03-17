using Abp.Configuration;
using Abp.Domain.Uow;
using Abp.Zero.AspNetCore;
using Microsoft.AspNetCore.Http;
using Magicodes.Admin.Authorization.Roles;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.MultiTenancy;

namespace Magicodes.Admin.Web.Identity
{
    public class SignInManager : AbpSignInManager<Tenant, Role, User>
    {
        public SignInManager(
            UserManager userManager,
            IHttpContextAccessor contextAccessor, 
            ISettingManager settingManager, 
            IUnitOfWorkManager unitOfWorkManager,
            IAbpZeroAspNetCoreConfiguration configuration) 
            : base(
                  userManager,
                  contextAccessor, 
                  settingManager, 
                  unitOfWorkManager,
                  configuration)
        {
        }
    }
}
