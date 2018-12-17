using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Extensions;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Cms.Host
{
    public class RouteProvider : IRouter
    {
        private readonly IRouter _mvcRoute;

        public RouteProvider(IServiceProvider services)
        {
            _mvcRoute = services.GetRequiredService<MvcRouteHandler>();
        }

        public async Task RouteAsync(RouteContext context)
        {
            var requestedUrl = context.HttpContext.Request.Path.Value.TrimEnd('/');

            //if (_urls.Contains(requestedUrl, StringComparer.OrdinalIgnoreCase))
            //{
            //    context.Handler = async ctx => {
            //        var response = ctx.Response;
            //        byte[] bytes = Encoding.ASCII.GetBytes($"This URL: {requestedUrl} is not available now");
            //        await response.Body.WriteAsync(bytes, 0, bytes.Length);
            //    };
            //}
            //return Task.CompletedTask;
            if (!requestedUrl.IsNullOrWhiteSpace() && requestedUrl.EndsWith(".html"))
            {
                //var split = requestedUrl.Split('/');
                //var title = secoend.Replace(".html", "");
                context.RouteData.Values["controller"] = "Home";
                context.RouteData.Values["action"] = "Detail";
                context.RouteData.Values["url"] = requestedUrl;
            }
            //...对请求路径进行一系列的判断
            //最后注入`MvcRouteHandler`示例执行`RouteAsync`方法，表示匹配成功
            await _mvcRoute.RouteAsync(context);
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }
    }
}
