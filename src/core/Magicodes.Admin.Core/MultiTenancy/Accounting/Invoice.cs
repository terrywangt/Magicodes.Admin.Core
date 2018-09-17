using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Magicodes.Admin.MultiTenancy.Accounting
{
    [Table("AppInvoices")]
    public class Invoice : Entity<int>
    {
        public string InvoiceNo { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string TenantLegalName { get; set; }

        public string TenantAddress { get; set; }

        public string TaxNumber { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// 银行账户
        /// </summary>
        public string BankAccount { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        public string Bank { get; set; }
    }
}
