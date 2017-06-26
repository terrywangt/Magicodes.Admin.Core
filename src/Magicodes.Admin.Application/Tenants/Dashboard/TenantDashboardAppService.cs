using Abp.Authorization;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Tenants.Dashboard.Dto;

namespace Magicodes.Admin.Tenants.Dashboard
{
    [AbpAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
    public class TenantDashboardAppService : AdminAppServiceBase, ITenantDashboardAppService
    {
        public GetMemberActivityOutput GetMemberActivity()
        {
            return new GetMemberActivityOutput
            (
                DashboardRandomDataGenerator.GenerateMemberActivities()
            );
        }

        public GetDashboardDataOutput GetDashboardData(GetDashboardDataInput input)
        {
            var output = new GetDashboardDataOutput
            {
                TotalProfit = DashboardRandomDataGenerator.GetRandomInt(5000, 9000),
                NewFeedbacks = DashboardRandomDataGenerator.GetRandomInt(1000, 5000),
                NewOrders = DashboardRandomDataGenerator.GetRandomInt(100, 900),
                NewUsers = DashboardRandomDataGenerator.GetRandomInt(50, 500),
                SalesSummary = DashboardRandomDataGenerator.GenerateSalesSummaryData(input.SalesSummaryDatePeriod),
                Expenses = DashboardRandomDataGenerator.GetRandomInt(5000, 10000),
                Growth = DashboardRandomDataGenerator.GetRandomInt(5000, 10000),
                Revenue = DashboardRandomDataGenerator.GetRandomInt(1000, 9000),
                TotalSales = DashboardRandomDataGenerator.GetRandomInt(10000, 90000),
                TransactionPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                NewVisitPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                BouncePercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                NetworkLoad = DashboardRandomDataGenerator.GetRandomArray(20, 8, 15),
                CpuLoad = DashboardRandomDataGenerator.GetRandomArray(20, 8, 15),
                LoadRate = DashboardRandomDataGenerator.GetRandomArray(20, 8, 15),
                TimeLineItems = DashboardRandomDataGenerator.GenerateTimeLineItems()
            };

            return output;
        }

        public GetSalesSummaryOutput GetSalesSummary(GetSalesSummaryInput input)
        {
            return new GetSalesSummaryOutput(DashboardRandomDataGenerator.GenerateSalesSummaryData(input.SalesSummaryDatePeriod));
        }

        public GetWorldMapOutput GetWorldMap(GetWorldMapInput input)
        {
            return new GetWorldMapOutput(DashboardRandomDataGenerator.GenerateWorldMapCountries());
        }

        public GetServerStatsOutput GetServerStats(GetServerStatsInput input)
        {
            return new GetServerStatsOutput
            {
                NetworkLoad = DashboardRandomDataGenerator.GetRandomArray(20, 8, 15),
                CpuLoad = DashboardRandomDataGenerator.GetRandomArray(20, 8, 15),
                LoadRate = DashboardRandomDataGenerator.GetRandomArray(20, 8, 15)
            };
        }

        public GetGeneralStatsOutput GetGeneralStats(GetGeneralStatsInput input)
        {
            return new GetGeneralStatsOutput
            {
                TransactionPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                NewVisitPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                BouncePercent = DashboardRandomDataGenerator.GetRandomInt(10, 100)
            };
        }
    }
}