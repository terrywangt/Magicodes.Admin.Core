namespace Magicodes.Admin.Web.Url
{
    public interface IWebUrlService
    {
        string WebSiteRootAddressFormat { get; }

        bool SupportsTenancyNameInUrl { get; }

        string GetSiteRootAddress(string tenancyName = null);
    }
}
