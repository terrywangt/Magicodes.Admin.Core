namespace Magicodes.Admin.Configuration.Host.Dto
{
    public class TenantManagementSettingsEditDto
    {
        public bool AllowSelfRegistration { get; set; }

        public bool IsNewRegisteredTenantActiveByDefault { get; set; }

        public bool UseCaptchaOnRegistration { get; set; }

        public int? DefaultEditionId { get; set; }

        public bool UseEnableTenantLogin { get; set; }
    }
}