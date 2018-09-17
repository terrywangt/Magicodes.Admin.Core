using System;
using System.Collections.Generic;

namespace Magicodes.Admin.MultiTenancy.Accounting.Dto
{
    public class InvoiceDto
    {
        public decimal Amount { get; set; }

        public string EditionDisplayName { get; set; }
        
        public string InvoiceNo { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string TenantLegalName { get; set; }

        public List<string> TenantAddress { get; set; }

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
        public string HostLegalName { get; set; }

        public List<string> HostAddress { get; set; }
    }
}