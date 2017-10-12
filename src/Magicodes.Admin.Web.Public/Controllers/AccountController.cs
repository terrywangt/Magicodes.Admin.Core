using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Abp;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using Microsoft.AspNetCore.Mvc;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Identity;
using Magicodes.Admin.MultiTenancy;
using Magicodes.Admin.Url;
using Magicodes.Admin.Web.Controllers;

namespace Magicodes.Admin.Web.Public.Controllers
{
    public class AccountController : AdminControllerBase
    {
        private readonly UserManager _userManager;
        private readonly SignInManager _signInManager;
        private readonly IWebUrlService _webUrlService;
        private readonly TenantManager _tenantManager;

        public AccountController(
            UserManager userManager,
            SignInManager signInManager,
            IWebUrlService webUrlService,
            TenantManager tenantManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webUrlService = webUrlService;
            _tenantManager = tenantManager;
        }

        public async Task<ActionResult> Login(string accessToken, string userId, string tenantId = "", string returnUrl = "")
        {
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(userId))
            {
                return await RedirectToExternalLoginPageAsync();
            }

            var targetTenantId = string.IsNullOrEmpty(tenantId) ? null : (int?)Convert.ToInt32(Base64Decode(tenantId));
            CurrentUnitOfWork.SetTenantId(targetTenantId);

            var targetUserId = Convert.ToInt64(Base64Decode(userId));

            var user = _userManager.GetUser(new UserIdentifier(targetTenantId, targetUserId));
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!user.SignInToken.Equals(accessToken) || !(user.SignInTokenExpireTimeUtc >= Clock.Now.ToUniversalTime()))
            {
                return RedirectToAction("Index", "Home");
            }

            CurrentUnitOfWork.SetTenantId(targetTenantId);
            await _signInManager.SignInAsync(user, false);

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> Logout()
        {
            var tenancyName = await GetCurrentTenancyName();
            var websiteAddress = _webUrlService.GetSiteRootAddress(tenancyName);
            var serverAddress = _webUrlService.GetServerRootAddress(tenancyName);

            await _signInManager.SignOutAsync();
            return Redirect(serverAddress.EnsureEndsWith('/') + "account/logout?returnUrl=" + websiteAddress);
        }

        private async Task<ActionResult> RedirectToExternalLoginPageAsync()
        {
            var tenancyName = await GetCurrentTenancyName();
            var serverAddress = _webUrlService.GetServerRootAddress(tenancyName);
            var websiteAddress = _webUrlService.GetSiteRootAddress(tenancyName);

            var originalReturnUrl = Request.Query.ContainsKey("ReturnUrl") ? Request.Query["ReturnUrl"].ToString() : "";
            var returnUrl = websiteAddress.EnsureEndsWith('/') + "account/login?returnUrl="+ websiteAddress.EnsureEndsWith('/')+ originalReturnUrl.TrimStart('/');
            return Redirect(serverAddress.EnsureEndsWith('/') + "account/login?ss=true&returnUrl=" + WebUtility.UrlEncode(returnUrl));
        }

        private async Task<string> GetCurrentTenancyName()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return "";
            }

            var tenant = await _tenantManager.GetByIdAsync(AbpSession.GetTenantId());
            return tenant.TenancyName;
        }

        private string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}