using Abp.AspNetZeroCore.Web.Authentication.External;
using Abp.AutoMapper;

namespace Magicodes.Admin.Web.Models.TokenAuth
{
    [AutoMapFrom(typeof(ExternalLoginProviderInfo))]
    public class ExternalLoginProviderInfoModel
    {
        public string Name { get; set; }

        public string ClientId { get; set; }
    }
}
