using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Magicodes.Admin;
using Magicodes.Admin.Authorization.Users;
using Microsoft.AspNetCore.Mvc;

namespace App.Host.Controllers
{
    [Route("api/[controller]")]
    public class TokenAuthController : AbpController
    {
        private readonly UserManager _userManager;
        private readonly ICacheManager _cacheManager;

        public TokenAuthController(UserManager userManager, ICacheManager cacheManager)
        {
            _userManager = userManager;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// 退出登陆
        /// </summary>
        /// <returns></returns>
        [HttpGet("LogOut")]
        [AbpAuthorize]
        public async Task LogOut()
        {
            if (AbpSession.UserId != null)
            {
                var tokenValidityKeyInClaims = User.Claims.First(c => c.Type == AppConsts.TokenValidityKey);
                await _userManager.RemoveTokenValidityKeyAsync(_userManager.GetUser(AbpSession.ToUserIdentifier()), tokenValidityKeyInClaims.Value);
                _cacheManager.GetCache(AppConsts.TokenValidityKey).Remove(tokenValidityKeyInClaims.Value);
            }
        }


    }
}
