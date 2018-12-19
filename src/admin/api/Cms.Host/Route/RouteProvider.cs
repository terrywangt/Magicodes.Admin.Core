using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Extensions;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Cms.Host.Route
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
            if (!requestedUrl.IsNullOrWhiteSpace() && requestedUrl.EndsWith(".html"))
            {
                var split = requestedUrl.Split('/');
                if (split.Length > 0)
                {
                    if (split[1].ToLower() == "article")
                    {
                        context.RouteData.Values["controller"] = "Article";
                        context.RouteData.Values["action"] = "Detail";
                        context.RouteData.Values["url"] = split.Last();
                    }
                }
            }

            await _mvcRoute.RouteAsync(context);
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }
    }
}