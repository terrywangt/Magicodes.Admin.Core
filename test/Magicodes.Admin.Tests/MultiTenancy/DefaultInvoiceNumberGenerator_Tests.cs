using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Timing;
using Magicodes.Admin.MultiTenancy.Accounting;
using Shouldly;

namespace Magicodes.Admin.Tests.MultiTenancy
{
    public class DefaultInvoiceNumberGenerator_Tests : AppTestBase
    {
        private readonly IInvoiceNumberGenerator _invoiceNumberGenerator;
        private readonly IRepository<Invoice> _invoiceRepository;

        public DefaultInvoiceNumberGenerator_Tests()
        {
            _invoiceNumberGenerator = Resolve<IInvoiceNumberGenerator>();
            _invoiceRepository = Resolve<IRepository<Invoice>>();
        }

        [MultiTenantFact]
        public async Task Should_Not_Generate_Same_Invoice_Number()
        {
            var invoiceCountToGenerate = 100;
            var invoiceNumbers = new List<string>();

            for (int i = 0; i < invoiceCountToGenerate; i++)
            {
                var invoiceNo = await _invoiceNumberGenerator.GetNewInvoiceNumber();
                invoiceNo.ShouldNotBeNullOrEmpty();
                invoiceNo.Length.ShouldBe(11);//Should be YYYYMM00001

                _invoiceRepository.Insert(new Invoice
                {
                    InvoiceNo = invoiceNo,
                    InvoiceDate = Clock.Now,
                    TenantAddress = "USA",
                    TenantLegalName = "AspNet Zero",
                    TenantTaxNo = "123456789"
                });

                invoiceNumbers.ShouldNotContain(invNo => invNo == invoiceNo);
                invoiceNumbers.Add(invoiceNo);
            }

            invoiceNumbers.Count.ShouldBe(invoiceCountToGenerate);
        }

        [MultiTenantFact]
        public async Task Should_Start_Over_InvoiceNo_When_Month_Changes()
        {
            _invoiceRepository.Insert(new Invoice
            {
                InvoiceNo = Clock.Now.Year + (Clock.Now.Month - 1).ToString("00") + "55555",
                InvoiceDate = Clock.Now,
                TenantAddress = "USA",
                TenantLegalName = "AspNet Zero",
                TenantTaxNo = "123456789"
            });

            var invoiceNo = await _invoiceNumberGenerator.GetNewInvoiceNumber();
            invoiceNo.ShouldBe(Clock.Now.Year + Clock.Now.Month.ToString("00") + "00001");
        }

        [MultiTenantFact]
        public async Task Should_Start_Over_InvoiceNo_When_Year_Changes()
        {
            _invoiceRepository.Insert(new Invoice
            {
                InvoiceNo = (Clock.Now.Year - 1) + (Clock.Now.Month).ToString("00") + "55555",
                InvoiceDate = Clock.Now,
                TenantAddress = "USA",
                TenantLegalName = "AspNet Zero",
                TenantTaxNo = "123456789"
            });

            var invoiceNo = await _invoiceNumberGenerator.GetNewInvoiceNumber();
            invoiceNo.ShouldBe(Clock.Now.Year + (Clock.Now.Month).ToString("00") + "00001");
        }

        [MultiTenantFact]
        public async Task Should_Handle_When_Invoice_Number_Exceeds_Defined_Range()
        {
            _invoiceRepository.Insert(new Invoice
            {
                InvoiceNo = Clock.Now.Year + Clock.Now.Month.ToString("00") + "99999",
                InvoiceDate = Clock.Now,
                TenantAddress = "USA",
                TenantLegalName = "AspNet Zero",
                TenantTaxNo = "123456789"
            });

            var invoiceNo = await _invoiceNumberGenerator.GetNewInvoiceNumber();
            invoiceNo.ShouldBe(Clock.Now.Year + (Clock.Now.Month).ToString("00") + "100000");

            _invoiceRepository.Insert(new Invoice
            {
                InvoiceNo = invoiceNo,
                InvoiceDate = Clock.Now,
                TenantAddress = "USA",
                TenantLegalName = "AspNet Zero",
                TenantTaxNo = "123456789"
            });

            invoiceNo = await _invoiceNumberGenerator.GetNewInvoiceNumber();
            invoiceNo.ShouldBe(Clock.Now.Year + (Clock.Now.Month).ToString("00") + "100001");
        }
    }
}
