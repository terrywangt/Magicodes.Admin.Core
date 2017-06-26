using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.Debugging;
using Magicodes.Admin.Identity;
using Magicodes.Admin.MultiTenancy;
using Magicodes.Admin.MultiTenancy.Dto;
using Magicodes.Admin.MultiTenancy.Payments;
using Magicodes.Admin.Security;
using Magicodes.Admin.Url;
using Magicodes.Admin.Web.Security.Recaptcha;
using System.Threading.Tasks;
using Magicodes.Admin.Editions;
using Magicodes.Admin.MultiTenancy.Payments.Dto;
using Magicodes.Admin.Web.Models.TenantRegistration;

namespace Magicodes.Admin.Web.Controllers
{
    public class TenantRegistrationController : AdminControllerBase
    {
        private readonly IMultiTenancyConfig _multiTenancyConfig;
        private readonly UserManager _userManager;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        private readonly LogInManager _logInManager;
        private readonly SignInManager _signInManager;
        private readonly IWebUrlService _webUrlService;
        private readonly ITenantRegistrationAppService _tenantRegistrationAppService;
        private readonly IPasswordComplexitySettingStore _passwordComplexitySettingStore;
        private readonly EditionManager _editionManager;

        public TenantRegistrationController(
            IMultiTenancyConfig multiTenancyConfig,
            UserManager userManager,
            AbpLoginResultTypeHelper abpLoginResultTypeHelper,
            LogInManager logInManager,
            SignInManager signInManager,
            IWebUrlService webUrlService,
            ITenantRegistrationAppService tenantRegistrationAppService,
            IPasswordComplexitySettingStore passwordComplexitySettingStore, 
            EditionManager editionManager)
        {
            _multiTenancyConfig = multiTenancyConfig;
            _userManager = userManager;
            _abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            _logInManager = logInManager;
            _signInManager = signInManager;
            _webUrlService = webUrlService;
            _tenantRegistrationAppService = tenantRegistrationAppService;
            _passwordComplexitySettingStore = passwordComplexitySettingStore;
            _editionManager = editionManager;
        }

        public async Task<ActionResult> SelectEdition()
        {
            var output = await _tenantRegistrationAppService.GetEditionsForSelect();
            var model = new EditionsSelectViewModel(output);

            return View(model);
        }

        public async Task<ActionResult> Register(int editionId, SubscriptionStartType subscriptionStartType, SubscriptionPaymentGatewayType? gateway = null, string paymentId = "")
        {
            CheckTenantRegistrationIsEnabled();

            var edition = await _tenantRegistrationAppService.GetEdition(editionId);

            var model = new TenantRegisterViewModel
            {
                PasswordComplexitySetting = await _passwordComplexitySettingStore.GetSettingsAsync(),
                EditionId = editionId,
                SubscriptionStartType = subscriptionStartType,
                Edition = edition,
                EditionPaymentType = EditionPaymentType.NewRegistration,
                Gateway = gateway,
                PaymentId = paymentId
            };

            ViewBag.UseCaptcha = UseCaptchaOnRegistration();

            return View(model);
        }

        [HttpPost]
        [UnitOfWork]
        public virtual async Task<ActionResult> Register(RegisterTenantInput model)
        {
            try
            {
                if (UseCaptchaOnRegistration())
                {
                    model.CaptchaResponse = HttpContext.Request.Form[RecaptchaValidator.RecaptchaResponseKey];
                }

                var result = await _tenantRegistrationAppService.RegisterTenant(model);

                CurrentUnitOfWork.SetTenantId(result.TenantId);

                var user = await _userManager.FindByNameAsync(AbpUserBase.AdminUserName);

                //Directly login if possible
                if (result.IsTenantActive && result.IsActive && !result.IsEmailConfirmationRequired &&
                    !_webUrlService.SupportsTenancyNameInUrl)
                {
                    var loginResult = await GetLoginResultAsync(user.UserName, model.AdminPassword, model.TenancyName);

                    if (loginResult.Result == AbpLoginResultType.Success)
                    {
                        await _signInManager.SignOutAsync();
                        await _signInManager.SignInAsync(loginResult.Identity, false);

                        SetTenantIdCookie(result.TenantId);

                        return Redirect(Url.Action("Index", "Home", new { area = "Admin" }));
                    }

                    Logger.Warn("New registered user could not be login. This should not be normally. login result: " + loginResult.Result);
                }

                //Show result page
                var resultModel = ObjectMapper.Map<TenantRegisterResultViewModel>(result);

                resultModel.TenantLoginAddress = _webUrlService.SupportsTenancyNameInUrl
                    ? _webUrlService.GetSiteRootAddress(model.TenancyName).EnsureEndsWith('/') + "Account/Login"
                    : "";

                return View("RegisterResult", resultModel);
            }
            catch (UserFriendlyException ex)
            {
                ViewBag.UseCaptcha = UseCaptchaOnRegistration();
                ViewBag.ErrorMessage = ex.Message;

                var edition = await _tenantRegistrationAppService.GetEdition(model.EditionId);
                var viewModel = new TenantRegisterViewModel
                {
                    PasswordComplexitySetting = await _passwordComplexitySettingStore.GetSettingsAsync(),
                    EditionId = model.EditionId,
                    SubscriptionStartType = model.SubscriptionStartType,
                    Edition = edition,
                    EditionPaymentType = EditionPaymentType.NewRegistration,
                    Gateway = model.Gateway,
                    PaymentId = model.PaymentId
                };

                return View("Register", viewModel);
            }
        }

        private bool IsSelfRegistrationEnabled()
        {
            return SettingManager.GetSettingValueForApplication<bool>(AppSettings.TenantManagement.AllowSelfRegistration);
        }

        private void CheckTenantRegistrationIsEnabled()
        {
            if (!IsSelfRegistrationEnabled())
            {
                throw new UserFriendlyException(L("SelfTenantRegistrationIsDisabledMessage_Detail"));
            }

            if (!_multiTenancyConfig.IsEnabled)
            {
                throw new UserFriendlyException(L("MultiTenancyIsNotEnabled"));
            }
        }

        private bool UseCaptchaOnRegistration()
        {
            if (DebugHelper.IsDebug)
            {
                return false;
            }

            return SettingManager.GetSettingValueForApplication<bool>(AppSettings.TenantManagement.UseCaptchaOnRegistration);
        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }
    }
}