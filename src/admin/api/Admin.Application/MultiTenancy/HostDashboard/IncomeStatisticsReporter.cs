using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Magicodes.Admin.MultiTenancy.HostDashboard.Dto;
using Magicodes.Admin.MultiTenancy.Payments;
using Magicodes.Admin.Core.Custom.LogInfos;

namespace Magicodes.Admin.MultiTenancy.HostDashboard
{
    public class IncomeStatisticsService : AdminDomainServiceBase, IIncomeStatisticsService
    {
        private readonly IRepository<SubscriptionPayment, long> _subscriptionPaymentRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<TransactionLog, long> _transactionLogRepository;


        public IncomeStatisticsService(IRepository<SubscriptionPayment, long> subscriptionPaymentRepository,
            IRepository<Tenant> tenantRepository,IRepository<TransactionLog, long> transactionLogRepository)
        {
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
            _tenantRepository = tenantRepository;
            _transactionLogRepository = transactionLogRepository;
        }

        /// <summary>
        /// 获取图表数据
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dateInterval"></param>
        /// <returns></returns>
        public async Task<List<IncomeStastistic>> GetIncomeStatisticsData(DateTime startDate, DateTime endDate,
            ChartDateInterval dateInterval)
        {
            List<IncomeStastistic> incomeStastistics;

            switch (dateInterval)
            {
                case ChartDateInterval.Transaction:
                    incomeStastistics = await GetTransactionIncomeStatisticsData(startDate, endDate);
                    break;
                case ChartDateInterval.Consumer:
                    incomeStastistics = await GetConsumerIncomeStatisticsData(startDate, endDate);
                    break;
                case ChartDateInterval.Order:
                    incomeStastistics = await GetOrderIncomeStatisticsData(startDate, endDate);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dateInterval), dateInterval, null);
            }

            incomeStastistics.ForEach(i =>
            {
                i.Label = i.Date.ToString(L("DateFormatShort"));
            });

            return incomeStastistics.OrderBy(i => i.Date).ToList();
        }

        /// <summary>
        /// 获取交易额统计
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<List<IncomeStastistic>> GetTransactionIncomeStatisticsData(DateTime startDate, DateTime endDate){

            List<IncomeStastistic> incomeStastisticlist = new List<IncomeStastistic>();
            for (int i = 0; i <= endDate.Day - startDate.Day; i++)
            {
                IncomeStastistic incomeStastistic = new IncomeStastistic
                {
                    Amount = await _subscriptionPaymentRepository.GetAll()
                                     .Where(p => p.CreationTime == startDate.AddDays(i))
                                     .Select(t => t.Amount)
                                     .SumAsync(),
                   
                    Date = startDate.AddDays(i),   
            };

                incomeStastisticlist.Add(incomeStastistic);
               
            }
          
                
            return incomeStastisticlist;
        }

        /// <summary>
        /// 获取用户统计
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<List<IncomeStastistic>> GetConsumerIncomeStatisticsData(DateTime startDate, DateTime endDate)
        {
            List<IncomeStastistic> incomeStastisticlist = new List<IncomeStastistic>();
            for (int i = 0; i <= endDate.Day - startDate.Day; i++)
            {
                IncomeStastistic incomeStastistic = new IncomeStastistic
                {
                    Amount = await _tenantRepository.GetAll()
                             .Where(p => p.CreationTime == startDate.AddDays(i))
                             .CountAsync(),
                  
                    Date = startDate.AddDays(i),
                };

                incomeStastisticlist.Add(incomeStastistic);
            }


            return incomeStastisticlist;
           
        }

        /// <summary>
        /// 获取订单统计
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<List<IncomeStastistic>> GetOrderIncomeStatisticsData(DateTime startDate, DateTime endDate)
        {
            List<IncomeStastistic> incomeStastisticlist = new List<IncomeStastistic>();
            for (int i = 0; i <= endDate.Day - startDate.Day; i++)
            {
                IncomeStastistic incomeStastistic = new IncomeStastistic
                {
                    Amount = await _transactionLogRepository.GetAll()
                    .Where(p=>p.CreationTime==startDate.AddDays(i))
                    .CountAsync(),
                   
                    Date = startDate.AddDays(i),
                };

                incomeStastisticlist.Add(incomeStastistic);
            }


            return incomeStastisticlist;
        }



    }
}