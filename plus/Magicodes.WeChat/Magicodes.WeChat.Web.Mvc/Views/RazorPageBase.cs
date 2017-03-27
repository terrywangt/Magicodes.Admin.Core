using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Magicodes.WeChat.Core;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace Magicodes.WeChat.Web.Mvc.Views
{
    public abstract class RazorPageBase<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }
        public string TmpData { get; set; }

        public const string ContentRootUrl = "/PlugIns/Magicodes.WeChat.Web.Mvc/wwwroot";
        public string SharedViewPathUrl = "~/wwwroot/PlugIns/Magicodes.WeChat.Web.Mvc/Views/Shared";
        protected RazorPageBase()
        {
            LocalizationSourceName = WeChatConsts.LocalizationSourceName;
        }
    }
}
