using Abp.AspNetCore.Mvc.ViewComponents;

namespace Magicodes.Admin.Web.Public.Views
{
    public abstract class AdminViewComponent : AbpViewComponent
    {
        protected AdminViewComponent()
        {
            LocalizationSourceName = AdminConsts.LocalizationSourceName;
        }
    }
}