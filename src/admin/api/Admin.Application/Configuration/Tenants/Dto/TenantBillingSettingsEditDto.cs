namespace Magicodes.Admin.Configuration.Tenants.Dto
{
    public class TenantBillingSettingsEditDto
    {
        /// <summary>
        /// 抬头名称
        /// </summary>
        public string LegalName { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 税号
        /// </summary>
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