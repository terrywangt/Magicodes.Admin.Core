using System;
using System.Linq;
using System.Threading.Tasks;
using Abp;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Magicodes.Admin.Editions;
using Magicodes.Admin.MultiTenancy;
using Magicodes.Admin.MultiTenancy.Dto;
using Magicodes.Admin.MultiTenancy.Payments;
using Magicodes.Admin.MultiTenancy.Payments.Cache;
using Magicodes.Admin.MultiTenancy.Payments.Dto;
using Magicodes.Admin.Web.Models.Payment;

namespace Magicodes.Admin.Web.Controllers
{
    public class PaymentController : AdminControllerBase
    {
        private readonly IPaymentAppService _paymentAppService;
        private readonly ITenantRegistrationAppService _tenantRegistrationAppService;
        private readonly IPaymentCache _paymentCache;
        private readonly TenantManager _tenantManager;
        private readonly EditionManager _editionManager;
        private readonly ISubscriptionAppService _subscriptionAppService;

        public PaymentController(
            IPaymentAppService paymentAppService,
            ITenantRegistrationAppService tenantRegistrationAppService,
            IPaymentCache paymentCache,
            TenantManager tenantManager,
            EditionManager editionManager, 
            ISubscriptionAppService subscriptionAppService)
        {
            _paymentAppService = paymentAppService;
            _tenantRegistrationAppService = tenantRegistrationAppService;
            _paymentCache = paymentCache;
            _tenantManager = tenantManager;
            _editionManager = editionManager;
            _subscriptionAppService = subscriptionAppService;
        }

        public async Task<IActionResult> Buy(int editionId, EditionPaymentType editionPaymentType, int? subscriptionStartType) //subscriptionStartType is used as int instead of SubscriptionStartType because MVC can not bind nullable enums
        {
            var edition = await _tenantRegistrationAppService.GetEdition(editionId);

            var model = new PaymentViewModel
            {
                Edition = edition,
                EditionPaymentType = editionPaymentType
            };

            if (subscriptionStartType.HasValue)
            {
                model.SubscriptionStartType = (SubscriptionStartType)subscriptionStartType.Value;
            }

            return View("Index", model);
        }

        public async Task<IActionResult> Upgrade(int upgradeEditionId, EditionPaymentType editionPaymentType)
        {
            return await UpgradeOrExtend(upgradeEditionId, editionPaymentType);
        }

        public async Task<IActionResult> Extend(int upgradeEditionId, EditionPaymentType editionPaymentType)
        {
            return await UpgradeOrExtend(upgradeEditionId, editionPaymentType);
        }

        [HttpPost]
        public async Task<CreatePaymentResponse> CreatePayment(CreatePaymentModel model)
        {
            var createPaymentResult = await _paymentAppService.CreatePayment(new CreatePaymentDto
            {
                PaymentPeriodType = model.PaymentPeriodType,
                EditionId = model.EditionId,
                EditionPaymentType = model.EditionPaymentType,
                SubscriptionPaymentGatewayType = model.Gateway
            });

            return createPaymentResult;
        }

        [HttpPost]
        public async Task<ActionResult> ExecutePayment(int editionId, PaymentPeriodType paymentPeriodType, SubscriptionPaymentGatewayType? gateway = null)
        {
            Check.NotNull(gateway, nameof(gateway));

            var edition = await _tenantRegistrationAppService.GetEdition(editionId);
            if (edition.IsFree)
            {
                throw new Exception("Can not execute payment for free editions!");
            }

            var paymentId = await ExecutePaymentAsync(editionId, paymentPeriodType, gateway.Value);
            return RedirectToAction("Register", "TenantRegistration", new
            {
                editionId = editionId,
                subscriptionStartType = SubscriptionStartType.Paid,
                gateway = gateway,
                paymentId = paymentId
            });
        }

        private async Task<string> ExecutePaymentAsync(int editionId, PaymentPeriodType paymentPeriodType, SubscriptionPaymentGatewayType gateway)
        {
            var data = Request.Form.ToDictionary(q => q.Key, q => string.Join(",", q.Value));

            var result = await _paymentAppService.ExecutePayment(new ExecutePaymentDto
            {
                EditionId = editionId,
                EditionPaymentType = EditionPaymentType.NewRegistration,
                Gateway = gateway,
                AdditionalData = data
            });

            var paymentId = result.GetId();
            _paymentCache.AddCacheItem(new PaymentCacheItem(gateway, paymentPeriodType, paymentId));

            return paymentId;
        }

        private async Task<IActionResult> UpgradeOrExtend(int upgradeEditionId, EditionPaymentType editionPaymentType)
        {
            var paymentInfo = await _paymentAppService.GetPaymentInfo(new PaymentInfoInput { UpgradeEditionId = upgradeEditionId });
            var edition = await _tenantRegistrationAppService.GetEdition(upgradeEditionId);

            if (editionPaymentType == EditionPaymentType.Upgrade && await UpgradeIsFree(upgradeEditionId))
            {
                await _subscriptionAppService.UpgradeTenantToEquivalentEdition(upgradeEditionId);
                return RedirectToAction("Index", "SubscriptionManagement", new { area = "Admin" });
            }

            var model = new PaymentViewModel
            {
                Edition = edition,
                AdditionalPrice = paymentInfo.AdditionalPrice,
                EditionPaymentType = editionPaymentType
            };

            return View("Index", model);
        }

        private async Task<bool> UpgradeIsFree(int upgradeEditionId)
        {
            var tenant = await _tenantManager.GetByIdAsync(AbpSession.GetTenantId());

            if (!tenant.EditionId.HasValue)
            {
                throw new Exception("Tenant must be assigned to an Edition in order to upgrade !");
            }

            var currentEdition = (SubscribableEdition)await _editionManager.GetByIdAsync(tenant.EditionId.Value);
            var targetEdition = (SubscribableEdition)await _editionManager.GetByIdAsync(upgradeEditionId);
            var bothEditionsAreFree = targetEdition.IsFree && currentEdition.IsFree;
            var bothEditionsHasSamePrice = currentEdition.HasSamePrice(targetEdition);
            return bothEditionsAreFree || bothEditionsHasSamePrice;
        }
    }
}