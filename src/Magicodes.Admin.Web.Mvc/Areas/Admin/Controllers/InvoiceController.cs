using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using Magicodes.Admin.MultiTenancy.Accounting;
using Magicodes.Admin.Web.Areas.Admin.Models.Accounting;
using Magicodes.Admin.Web.Controllers;

namespace Magicodes.Admin.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InvoiceController : AdminControllerBase
    {
        private readonly IInvoiceAppService _invoiceAppService;

        public InvoiceController(IInvoiceAppService invoiceAppService)
        {
            _invoiceAppService = invoiceAppService;
        }


        [HttpGet]
        public async Task<ActionResult> Index(long paymentId)
        {
            var invoice = await _invoiceAppService.GetInvoiceInfo(new EntityDto<long>(paymentId));
            var model = new InvoiceViewModel
            {
                Invoice = invoice
            };

            return View(model);
        }
    }
}