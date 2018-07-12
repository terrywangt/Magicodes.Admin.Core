using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Magicodes.Admin.MultiTenancy.Accounting.Dto;

namespace Magicodes.Admin.MultiTenancy.Accounting
{
    public interface IInvoiceAppService
    {
        Task<InvoiceDto> GetInvoiceInfo(EntityDto<long> input);

        Task CreateInvoice(CreateInvoiceDto input);
    }
}
