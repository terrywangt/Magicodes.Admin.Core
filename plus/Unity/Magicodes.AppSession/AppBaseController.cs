using Magicodes.Admin.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Magicodes.AppSession
{
    [AllowAnonymous]
    public class AppBaseController : AdminPluginControllerBase
    {
        public static Dictionary<RequestTypes, Action<ActionExecutingContext, AppBaseController>> RequestTypeActions { get; set; }
        public static Dictionary<RequestTypes, string> LoginUrls { get; set; }
        public IHostingEnvironment HostingEnvironment { get; set; }
        public IAppSession AppSession { get; set; }
        static AppBaseController()
        {
            RequestTypeActions = new Dictionary<RequestTypes, Action<ActionExecutingContext, AppBaseController>>();
            LoginUrls = new Dictionary<RequestTypes, string>();
        }
        public AppBaseController(string localizationSourceName) : base(localizationSourceName)
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var requestType = RequestTypes.Default;
            var userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString().ToLower();
            //是否来自微信端请求
            if (userAgent.Contains("micromessenger"))
            {
                requestType = RequestTypes.WeChat;
            }
            else if (userAgent.Contains("android"))
            {
                requestType = RequestTypes.Android;
            }
            else if (userAgent.Contains("iphone"))
            {
                requestType = RequestTypes.IOS;
            }

            if (HostingEnvironment != null && HostingEnvironment.IsDevelopment() && !string.IsNullOrWhiteSpace(context.HttpContext.Request.Query["agent"]))
            {
                var agent = context.HttpContext.Request.Query["agent"];
                requestType = (RequestTypes)Enum.Parse(typeof(RequestTypes), agent);
            }
            //执行相应的请求处理逻辑
            if (RequestTypeActions.ContainsKey(requestType))
            {
                RequestTypeActions[requestType].Invoke(context, this);
            }
        }
    }
}
