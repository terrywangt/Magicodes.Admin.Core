using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace Magicodes.Home.Views
{
    public abstract class RazorPageBase<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }
        public string TmpData { get; set; }

        public string ContentRootUrl { get; private set; }
        public string SharedViewPathUrl { get; private set; }
        protected RazorPageBase()
        {
            LocalizationSourceName = HomeConsts.LocalizationSourceName;
            ContentRootUrl = "/PlugIns/Magicodes.Home/wwwroot";
            SharedViewPathUrl = "~/wwwroot/PlugIns/Magicodes.Home/Views/Shared";
        }
    }
}
