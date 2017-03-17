using Abp.Dependency;
using Abp.Extensions;
using Abp.MultiTenancy;
using Microsoft.AspNetCore.Hosting;
using Magicodes.Admin.Web.Configuration;

namespace Magicodes.Admin.Web.Url //TODO: Rename namespaces
{
    public class WebUrlService : IWebUrlService, ITransientDependency
    {
        public const string TenancyNamePlaceHolder = "{TENANCY_NAME}";

        public string WebSiteRootAddressFormat
        {
            get
            {
                return _hostingEnvironment.GetAppConfiguration()["App:WebSiteRootAddress"] ?? "http://localhost:62114/";
            }
        }

        public bool SupportsTenancyNameInUrl
        {
            get
            {
                var siteRootFormat = WebSiteRootAddressFormat;
                return !siteRootFormat.IsNullOrEmpty() && siteRootFormat.Contains(TenancyNamePlaceHolder);
            }
        }

        private readonly ITenantCache _tenantCache;
        private readonly IHostingEnvironment _hostingEnvironment;

        public WebUrlService(
            IHostingEnvironment hostingEnvironment,
            ITenantCache tenantCache
            )
        {
            _hostingEnvironment = hostingEnvironment;
            _tenantCache = tenantCache;
        }

        public string GetSiteRootAddress(string tenancyName = null)
        {
            var siteRootFormat = WebSiteRootAddressFormat;

            if (!siteRootFormat.Contains(TenancyNamePlaceHolder))
            {
                return siteRootFormat;
            }

            if (siteRootFormat.Contains(TenancyNamePlaceHolder + "."))
            {
                siteRootFormat = siteRootFormat.Replace(TenancyNamePlaceHolder + ".", TenancyNamePlaceHolder);
            }

            if (tenancyName.IsNullOrEmpty())
            {
                return siteRootFormat.Replace(TenancyNamePlaceHolder, "");
            }

            return siteRootFormat.Replace(TenancyNamePlaceHolder, tenancyName + ".");
        }
    }
}