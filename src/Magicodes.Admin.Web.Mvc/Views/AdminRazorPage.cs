using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace Magicodes.Admin.Web.Views
{
    public abstract class AdminRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected AdminRazorPage()
        {
            LocalizationSourceName = AdminConsts.LocalizationSourceName;
        }
    }
}
