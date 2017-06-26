using Abp.Application.Services;
using Magicodes.Admin.Tenants.Dashboard.Dto;

namespace Magicodes.Admin.Tenants.Dashboard
{
    public interface ITenantDashboardAppService : IApplicationService
    {
        GetMemberActivityOutput GetMemberActivity();

        GetDashboardDataOutput GetDashboardData(GetDashboardDataInput input);

        GetSalesSummaryOutput GetSalesSummary(GetSalesSummaryInput input);

        GetWorldMapOutput GetWorldMap(GetWorldMapInput input);

        GetServerStatsOutput GetServerStats(GetServerStatsInput input);

        GetGeneralStatsOutput GetGeneralStats(GetGeneralStatsInput input);
    }
}
