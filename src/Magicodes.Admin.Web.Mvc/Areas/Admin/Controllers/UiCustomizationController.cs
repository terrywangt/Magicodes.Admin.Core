using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.Web.Areas.Admin.Models.UiCustomization;
using Magicodes.Admin.Web.Controllers;

namespace Magicodes.Admin.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AbpMvcAuthorize]
    public class UiCustomizationController : AdminControllerBase
    {
        private readonly IUiCustomizationSettingsAppService _uiCustomizationAppService;

        public UiCustomizationController(IUiCustomizationSettingsAppService uiCustomizationAppService)
        {
            _uiCustomizationAppService = uiCustomizationAppService;
        }

        public async Task<ActionResult> Index()
        {
            var model = new UiCustomizationViewModel
            {
                Settings = await _uiCustomizationAppService.GetUiManagementSettings(),
                HasUiCustomizationPagePermission = await PermissionChecker.IsGrantedAsync(AppPermissions.Pages_Administration_UiCustomization)
            };

            return View(model);
        }
    }
}