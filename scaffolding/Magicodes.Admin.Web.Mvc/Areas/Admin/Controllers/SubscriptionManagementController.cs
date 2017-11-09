using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Editions;
using Magicodes.Admin.MultiTenancy.Dto;
using Magicodes.Admin.MultiTenancy.Payments;
using Magicodes.Admin.MultiTenancy.Payments.Dto;
using Magicodes.Admin.Web.Areas.Admin.Models.Editions;
using Magicodes.Admin.Web.Areas.Admin.Models.SubscriptionManagement;
using Magicodes.Admin.Web.Controllers;
using Magicodes.Admin.Web.Session;
using PaymentViewModel = Magicodes.Admin.Web.Models.Payment.PaymentViewModel;

namespace Magicodes.Admin.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AbpMvcAuthorize(AppPermissions.Pages_Administration_Tenant_SubscriptionManagement)]
    public class SubscriptionManagementController : AdminControllerBase
    {
        private readonly IPerRequestSessionCache _sessionCache;
        private readonly IPaymentAppService _paymentAppService;

        public SubscriptionManagementController(
            IPerRequestSessionCache sessionCache,
            IPaymentAppService paymentAppService
        )
        {
            _sessionCache = sessionCache;
            _paymentAppService = paymentAppService;
        }

        public async Task<ActionResult> Index()
        {
            var loginInfo = await _sessionCache.GetCurrentLoginInformationsAsync();
            var model = new SubscriptionDashboardViewModel
            {
                LoginInformations = loginInfo
            };

            return View(model);
        }

        public async Task<ActionResult> Payment(int? upgradeEditionId, EditionPaymentType editionPaymentType)
        {
            var paymentInfo = await _paymentAppService.GetPaymentInfo(new PaymentInfoInput { UpgradeEditionId = upgradeEditionId });

            return View("~/Views/Payment/Index.cshtml", new PaymentViewModel
            {
                Edition = paymentInfo.Edition,
                AdditionalPrice = paymentInfo.AdditionalPrice,
                EditionPaymentType = editionPaymentType
            });
        }

        [HttpPost]
        public async Task<ActionResult> PaymentResult(PaymentResultViewModel model)
        {
            var data = Request.Form.ToDictionary(q => q.Key, q => string.Join(",", q.Value));
            var executePaymentDto = ObjectMapper.Map<ExecutePaymentDto>(model);
            executePaymentDto.AdditionalData = data;

            await _paymentAppService.ExecutePayment(executePaymentDto);

            return RedirectToAction("Index");
        }
    }
}