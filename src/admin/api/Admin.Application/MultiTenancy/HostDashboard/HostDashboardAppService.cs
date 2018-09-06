using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Timing;
using Microsoft.EntityFrameworkCore;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.MultiTenancy.HostDashboard.Dto;
using Magicodes.Admin.MultiTenancy.Payments;
using Magicodes.Admin.Core.Custom.LogInfos;

namespace Magicodes.Admin.MultiTenancy.HostDashboard
{
    [DisableAuditing]
    [AbpAuthorize(AppPermissions.Pages_Administration_Host_Dashboard)]
    public class HostDashboardAppService : AdminAppServiceBase, IHostDashboardAppService
    {
        private const int SubscriptionEndAlertDayCount = 30;
        private const int MaxExpiringTenantsShownCount = 10;
        private const int MaxRecentTenantsShownCount = 10;
        private const int RecentTenantsDayCount = 7;

        private readonly IRepository<SubscriptionPayment, long> _subscriptionPaymentRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IIncomeStatisticsService _incomeStatisticsService;
        private readonly IRepository<TransactionLog, long> _transactionLogRepository;


        public HostDashboardAppService(IRepository<SubscriptionPayment, long> subscriptionPaymentRepository,
            IRepository<Tenant> tenantRepository,
            IIncomeStatisticsService incomeStatisticsService, IRepository<TransactionLog, long> transactionLogRepository)
        {
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
            _tenantRepository = tenantRepository;
            _incomeStatisticsService = incomeStatisticsService;
            _transactionLogRepository = transactionLogRepository;
        }

        public async Task<HostDashboardData> GetDashboardStatisticsData(GetDashboardDataInput input)
        {
            var subscriptionEndDateEndUtc = Clock.Now.ToUniversalTime().AddDays(SubscriptionEndAlertDayCount);
            var subscriptionEndDateStartUtc = Clock.Now.ToUniversalTime();
            var tenantCreationStartDate = Clock.Now.ToUniversalTime().AddDays(-RecentTenantsDayCount);

            return new HostDashboardData
            {
                NewGoodsCount = await GetGoodsCountByDate(),
                NewOrdersCount = await GetOrdersCountByDate(),
                NewTenantsCount = await GetTenantsCountByDate(),
                NewSubscriptionAmount = await GetSubscriptionAmount(),
                NewGoodsratio = await GetGoodsCountByDate()!=0? await GetGoodsCountByDate(input.StartDate, input.EndDate) * 100 % await GetGoodsCountByDate():0,
                NewOrdersratio = await GetOrdersCountByDate() != 0 ? await GetOrdersCountByDate(input.StartDate, input.EndDate) * 100 % await GetOrdersCountByDate() : 0,
                SubscriptionAmountratio = await GetSubscriptionAmount() != 0 ? Convert.ToInt32(await GetNewSubscriptionAmount(input.StartDate, input.EndDate) * 100 % await GetSubscriptionAmount()) : 0,
                NewTenantsratio =await GetTenantsCountByDate() != 0 ? await GetTenantsCountByDate(input.StartDate, input.EndDate) * 100 % await GetTenantsCountByDate() : 0,
                
                   

                IncomeStatistics = await _incomeStatisticsService.GetIncomeStatisticsData(input.StartDate, input.EndDate, input.IncomeStatisticsDateInterval),
                EditionStatistics = await GetEditionTenantStatisticsData(input.StartDate, input.EndDate),
                ExpiringTenants = await GetExpiringTenantsData(subscriptionEndDateStartUtc, subscriptionEndDateEndUtc, MaxExpiringTenantsShownCount),
                RecentTenants = await GetRecentTenantsData(tenantCreationStartDate, MaxRecentTenantsShownCount),
                MaxExpiringTenantsShownCount = MaxExpiringTenantsShownCount,
                MaxRecentTenantsShownCount = MaxRecentTenantsShownCount,
                SubscriptionEndAlertDayCount = SubscriptionEndAlertDayCount,
                RecentTenantsDayCount = RecentTenantsDayCount,
                SubscriptionEndDateStart = subscriptionEndDateStartUtc,
                SubscriptionEndDateEnd = subscriptionEndDateEndUtc,
                TenantCreationStartDate = tenantCreationStartDate
            };
        }

        public async Task<GetIncomeStatisticsDataOutput> GetIncomeStatistics(GetIncomeStatisticsDataInput input)
        {
            return new GetIncomeStatisticsDataOutput(await _incomeStatisticsService.GetIncomeStatisticsData(input.StartDate, input.EndDate, input.IncomeStatisticsDateInterval));
        }
 
        public async Task<GetEditionTenantStatisticsOutput> GetEditionTenantStatistics(GetEditionTenantStatisticsInput input)
        {
            return new GetEditionTenantStatisticsOutput(await GetEditionTenantStatisticsData(input.StartDate, input.EndDate));
        }

        private async Task<List<TenantEdition>> GetEditionTenantStatisticsData(DateTime startDate, DateTime endDate)
        {
            return await _tenantRepository.GetAll()
                .Where(t => t.EditionId.HasValue &&
                            t.IsActive &&
                            t.CreationTime >= startDate &&
                            t.CreationTime <= endDate)
                .GroupBy(t => t.Edition)
                .Select(t => new TenantEdition
                {
                    Label = t.Key.DisplayName,
                    Value = t.Count()
                })
                .OrderBy(t => t.Label)
                .ToListAsync();
        }
        /// <summary>
        /// 新增交易金额
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private async Task<decimal> GetNewSubscriptionAmount(DateTime startDate, DateTime endDate)
        {
            return await _subscriptionPaymentRepository.GetAll()
                .Where(s => s.CreationTime >= startDate &&
                            s.CreationTime <= endDate &&
                            s.Status == SubscriptionPaymentStatus.Completed)
                .Select(x => x.Amount)
                .DefaultIfEmpty(0)
                .SumAsync();
        }
        /// <summary>
        /// 交易总额
        /// </summary>
        /// <returns></returns>
        private async Task<decimal> GetSubscriptionAmount()
        {
            return await _subscriptionPaymentRepository.GetAll()
                .Where(s => s.Status == SubscriptionPaymentStatus.Completed)
                .Select(x => x.Amount)
                .DefaultIfEmpty(0)
                .SumAsync();
        }
        /// <summary>
        /// 新增租户数
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>

        private async Task<int> GetTenantsCountByDate(DateTime startDate, DateTime endDate)
        {
            return await _tenantRepository.GetAll()
                .Where(t => t.CreationTime >= startDate && t.CreationTime <= endDate)
                .CountAsync();
        }
        /// <summary>
        /// 总租户数
        /// </summary>
        /// <returns></returns>
        private async Task<int> GetTenantsCountByDate()
        {
            return await _tenantRepository.GetAll().CountAsync();
        }
        /// <summary>
        /// 获取新增订单数
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private async Task<int> GetOrdersCountByDate(DateTime startDate, DateTime endDate)
        {
            return await _transactionLogRepository.GetAll()
                .Where(t => t.CreationTime >= startDate && t.CreationTime <= endDate)
                .CountAsync();
        }
        /// <summary>
        /// 总订单数
        /// </summary>
        /// <returns></returns>
        private async Task<int> GetOrdersCountByDate()
        {
            return await _transactionLogRepository.GetAll().CountAsync();
        }

        /// <summary>
        /// 获取新增商品数
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private async Task<int> GetGoodsCountByDate(DateTime startDate, DateTime endDate)
        {
            return await _tenantRepository.GetAll()
                .Where(t => t.CreationTime >= startDate && t.CreationTime <= endDate)
                .CountAsync();
        }
        /// <summary>
        /// 总商品数
        /// </summary>
        /// <returns></returns>
        private async Task<int> GetGoodsCountByDate()
        {
            return await _tenantRepository.GetAll().CountAsync();
        }

        /// <summary>
        /// 获取待办事项
        /// </summary>
        /// <returns></returns>
     public async Task<GetPendsOutput> GetPendsData()
        {
            GetPendsOutput getPendsOutput = new GetPendsOutput
            {
                Consult = 10,
                Delivery =2,
                Obligation = 598,
                Rejectedgoods = 25,
                Stock = 3,
            };

            return getPendsOutput;
        }

        /// <summary>
        /// 获取热销商品
        /// </summary>
        /// <returns></returns>
        public async Task<List<GetHotGoodsOutput>> GetHotGoodsData(DateTime startDate, DateTime endDate)
        {
            List<GetHotGoodsOutput> getHotGoodsOutputs = new List<GetHotGoodsOutput>();
            GetHotGoodsOutput GetHotGoodsOutput1 = new GetHotGoodsOutput() {
                 goodname="加多宝",
                 goodtype="饮料",
                 sales=520943,
            };

            GetHotGoodsOutput GetHotGoodsOutput2 = new GetHotGoodsOutput()
            {
                goodname = "韦德超音速粉色球鞋",
                goodtype = "服饰",
                sales = 20653,
            };

            GetHotGoodsOutput GetHotGoodsOutput3 = new GetHotGoodsOutput()
            {
                goodname = "韩版简约手链",
                goodtype = "首饰",
                sales = 93601,
            };

            GetHotGoodsOutput GetHotGoodsOutput4 = new GetHotGoodsOutput()
            {
                goodname = "小茗同学",
                goodtype = "饮料",
                sales = 867080,
            };

            GetHotGoodsOutput GetHotGoodsOutput5 = new GetHotGoodsOutput()
            {
                goodname = "oppo find5",
                goodtype = "电子产品",
                sales = 90825,
            };

            getHotGoodsOutputs.Add(GetHotGoodsOutput1);
            getHotGoodsOutputs.Add(GetHotGoodsOutput2);
            getHotGoodsOutputs.Add(GetHotGoodsOutput3);
            getHotGoodsOutputs.Add(GetHotGoodsOutput4);
            getHotGoodsOutputs.Add(GetHotGoodsOutput5);

            getHotGoodsOutputs.OrderBy(p=>p.sales); 
            return getHotGoodsOutputs;  
        }

        /// <summary>
        /// 获取系统动态
        /// </summary>
        /// <returns></returns>
        private async Task<List<GetDynamiclistOutput>> GetDynamiclistData()
        {
            List<GetDynamiclistOutput> getDynamiclistOutputs = new List<GetDynamiclistOutput>();
            GetDynamiclistOutput getDynamiclistOutput1 = new GetDynamiclistOutput
            {
                 Context="增加新用户流星",
                 Days="12",
                 Mothdate="2018/07",
                 
            };
            return null;

        }

     

        private async Task<List<ExpiringTenant>> GetExpiringTenantsData(DateTime subscriptionEndDateStartUtc, DateTime subscriptionEndDateEndUtc, int? maxExpiringTenantsShownCount = null)
        {
            var query = _tenantRepository.GetAll().Where(t =>
                    t.SubscriptionEndDateUtc.HasValue &&
                    t.SubscriptionEndDateUtc.Value >= subscriptionEndDateStartUtc &&
                    t.SubscriptionEndDateUtc.Value <= subscriptionEndDateEndUtc)
                .Select(t => new ExpiringTenant
                {
                    TenantName = t.Name,
                    RemainingDayCount = Convert.ToInt32(t.SubscriptionEndDateUtc.Value.Subtract(subscriptionEndDateStartUtc).TotalDays)
                });

            if (maxExpiringTenantsShownCount.HasValue)
            {
                query = query.Take(maxExpiringTenantsShownCount.Value);
            }

            return await query.OrderBy(t => t.RemainingDayCount).ThenBy(t => t.TenantName).ToListAsync();
        }

        private async Task<List<RecentTenant>> GetRecentTenantsData(DateTime creationDateStart, int? maxRecentTenantsShownCount = null)
        {
            var query = _tenantRepository.GetAll()
                .Where(t => t.CreationTime >= creationDateStart)
                .OrderByDescending(t => t.CreationTime);

            if (maxRecentTenantsShownCount.HasValue)
            {
                query = (IOrderedQueryable<Tenant>)query.Take(maxRecentTenantsShownCount.Value);
            }

            return await query.Select(t => ObjectMapper.Map<RecentTenant>(t)).ToListAsync();
        }
    }
}