using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNet.Identity;

namespace Magicodes.Admin.Web.Controllers
{
    public abstract class AdminControllerBase : AbpController
    {
        protected AdminControllerBase()
        {
            LocalizationSourceName = AdminConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}