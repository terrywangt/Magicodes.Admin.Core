using System;
using System.Collections.Generic;

namespace Magicodes.Admin.MultiTenancy.HostDashboard.Dto
{
    public class HostDashboardData
    {
        /// <summary>
        /// 总租户数
        /// </summary>
        public int NewTenantsCount { get; set; }
        /// <summary>
        /// 租户新增比率
        /// </summary>
        public int NewTenantsratio { get; set; }

        /// <summary>
        /// 总交易金额
        /// </summary>
        public decimal NewSubscriptionAmount { get; set; }
        /// <summary>
        /// 交易额新增比率
        /// </summary>
        public int SubscriptionAmountratio { get; set; }
        /// <summary>
        /// 总订单数
        /// </summary>
        public int NewOrdersCount { get; set; }
        /// <summary>
        /// 订单数新增比率
        /// </summary>
        public int NewOrdersratio { get; set; }
        /// <summary>
        /// 总商品数
        /// </summary>
        public int NewGoodsCount { get; set; }

        /// <summary>
        /// 商品数新增比率
        /// </summary>
        public int NewGoodsratio { get; set; }

        /// <summary>
        /// 系统动态数据
        /// </summary>
        public GetDynamiclistOutput GetDynamiclistOutput { get; set; }


        public List<IncomeStastistic> IncomeStatistics { get; set; }
        public List<TenantEdition> EditionStatistics { get; set; }
        public List<ExpiringTenant> ExpiringTenants { get; set; }
        public List<RecentTenant> RecentTenants { get; set; }
        public int MaxExpiringTenantsShownCount { get; set; }
        public int MaxRecentTenantsShownCount { get; set; }
        public int SubscriptionEndAlertDayCount { get; set; }
        public int RecentTenantsDayCount { get; set; }
        public DateTime SubscriptionEndDateStart { get; set; }
        public DateTime SubscriptionEndDateEnd { get; set; }
        public DateTime TenantCreationStartDate { get; set; }
    }
}

