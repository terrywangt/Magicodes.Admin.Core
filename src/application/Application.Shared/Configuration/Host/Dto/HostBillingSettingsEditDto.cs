namespace Magicodes.Admin.Configuration.Host.Dto
{
    public class HostBillingSettingsEditDto
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
        public string Dutyparagraph { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 账户
        /// </summary>
        public  string Account { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        public string Openingbank { get; set; }


    }
}