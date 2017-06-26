using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magicodes.Admin.MultiTenancy.HostDashboard.Dto;

namespace Magicodes.Admin.MultiTenancy.HostDashboard
{
    public interface IIncomeStatisticsService
    {
        Task<List<IncomeStastistic>> GetIncomeStatisticsData(DateTime startDate, DateTime endDate,
            ChartDateInterval dateInterval);
    }
}