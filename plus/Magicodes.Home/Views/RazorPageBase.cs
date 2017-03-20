using Abp.AspNetCore.Mvc.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magicodes.Home.Views
{
    public abstract class RazorPageBase<TModel> : AbpRazorPage<TModel>
    {
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
