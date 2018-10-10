using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.Editions;
using Magicodes.Admin.MultiTenancy.Accounting.Dto;
using Magicodes.Admin.MultiTenancy.Payments;

namespace Magicodes.Admin.MultiTenancy.Accounting
{
    public class InvoiceAppService : AdminAppServiceBase, IInvoiceAppService
    {
        private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;
        private readonly IInvoiceNumberGenerator _invoiceNumberGenerator;
        private readonly EditionManager _editionManager;
        private readonly IRepository<Invoice> _invoiceRepository;

        public InvoiceAppService(
            ISubscriptionPaymentRepository subscriptionPaymentRepository,
            IInvoiceNumberGenerator invoiceNumberGenerator,
            EditionManager editionManager,
            IRepository<Invoice> invoiceRepository)
        {
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
            _invoiceNumberGenerator = invoiceNumberGenerator;
            _editionManager = editionManager;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<InvoiceDto> GetInvoiceInfo(EntityDto<long> input)
        {
            var payment = await _subscriptionPaymentRepository.GetAsync(input.Id);

            if (string.IsNullOrEmpty(payment.InvoiceNo))
            {
                throw new Exception("There is no invoice for this payment !");
            }

            if (payment.TenantId != AbpSession.GetTenantId())
            {
                throw new UserFriendlyException(L("ThisInvoiceIsNotYours"));
            }

            var invoice = await _invoiceRepository.FirstOrDefaultAsync(b => b.InvoiceNo == payment.InvoiceNo);
            if (invoice == null)
            {
                throw new UserFriendlyException();
            }

            var edition = await _editionManager.FindByIdAsync(payment.EditionId);
            var hostAddress = await SettingManager.GetSettingValueAsync(AppSettings.HostManagement.BillingAddress);

            return new InvoiceDto
            {
                InvoiceNo = payment.InvoiceNo,
                InvoiceDate = invoice.InvoiceDate,
                Amount = payment.Amount,
                EditionDisplayName = edition.DisplayName,

                HostAddress = hostAddress.Replace("\r\n", "|").Split('|').ToList(),
                HostLegalName = await SettingManager.GetSettingValueAsync(AppSettings.HostManagement.BillingLegalName),

                TenantAddress = invoice.TenantAddress.Replace("\r\n", "|").Split('|').ToList(),
                TenantLegalName = invoice.TenantLegalName,
                Bank = invoice.Bank,
                BankAccount = invoice.Bank,
                TaxNumber = invoice.TenantTaxNo,
                Contact = invoice.Contact,
            };
        }

        [UnitOfWork(IsolationLevel.ReadUncommitted)]
        public async Task CreateInvoice(CreateInvoiceDto input)
        {
            var payment = await _subscriptionPaymentRepository.GetAsync(input.SubscriptionPaymentId);
            if (!string.IsNullOrEmpty(payment.InvoiceNo))
            {
                throw new Exception("Invoice is already generated for this payment.");
            }

            var invoiceNo = await _invoiceNumberGenerator.GetNewInvoiceNumber();

            var tenantLegalName = await SettingManager.GetSettingValueAsync(AppSettings.TenantManagement.BillingLegalName);
            var tenantAddress = await SettingManager.GetSettingValueAsync(AppSettings.TenantManagement.BillingAddress);
            var tenantTaxNumber = await SettingManager.GetSettingValueAsync(AppSettings.TenantManagement.BillingTaxNumber);
            var tenantContact = await SettingManager.GetSettingValueAsync(AppSettings.TenantManagement.BillingContact);
            var tenantBank = await SettingManager.GetSettingValueAsync(AppSettings.TenantManagement.BillingBank);
            var tenantBankAccount = await SettingManager.GetSettingValueAsync(AppSettings.TenantManagement.BillingBankAccount);

            if (string.IsNullOrEmpty(tenantLegalName) || string.IsNullOrEmpty(tenantAddress) || string.IsNullOrEmpty(tenantTaxNumber) || string.IsNullOrEmpty(tenantContact) || string.IsNullOrEmpty(tenantBank) || string.IsNullOrEmpty(tenantBankAccount))
            {
                throw new UserFriendlyException(L("InvoiceInfoIsMissingOrNotCompleted"));
            }

            await _invoiceRepository.InsertAsync(new Invoice
            {
                InvoiceNo = invoiceNo,
                InvoiceDate = Clock.Now,
                TenantLegalName = tenantLegalName,
                TenantAddress = tenantAddress,
                Bank = tenantBank,
                BankAccount = tenantBankAccount,
                Contact = tenantContact,
                TenantTaxNo = tenantTaxNumber,
            });

            payment.InvoiceNo = invoiceNo;
        }
    }
}