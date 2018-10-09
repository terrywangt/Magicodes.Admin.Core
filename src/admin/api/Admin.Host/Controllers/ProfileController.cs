using Abp.AspNetCore.Mvc.Authorization;
using Magicodes.Admin.Storage;

namespace Magicodes.Admin.Web.Controllers
{
    [AbpMvcAuthorize]
    public class ProfileController : ProfileControllerBase
    {
        public ProfileController(ITempFileCacheManager tempFileCacheManager) :
            base(tempFileCacheManager)
        {
        }
    }
}