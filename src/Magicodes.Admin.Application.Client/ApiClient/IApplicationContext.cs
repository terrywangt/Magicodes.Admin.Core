using Abp.Web.Models.AbpUserConfiguration;
using JetBrains.Annotations;
using Magicodes.Admin.Sessions.Dto;

namespace Magicodes.Admin.ApiClient
{
    public interface IApplicationContext
    {
        [CanBeNull]
        TenantInformation CurrentTenant { get; }

        AbpUserConfigurationDto Configuration { get; set; }

        GetCurrentLoginInformationsOutput LoginInfo { get; set; }

        void SetAsHost();

        void SetAsTenant(string tenancyName, int tenantId);
    }
}