using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Magicodes.AppSession
{
    public class AppTenantManager : IAppTenantManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppTenantManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public void Initialize()
        {
        }

        public int GetTenantId()
        {
            var context = _httpContextAccessor.HttpContext;
            //租户Id
            var tenantId = default(int);
            //请求参数中的租户Id
            int reqTennantId;

            #region 获取请求参数中的租户Id

            if (!string.IsNullOrWhiteSpace(context.Request.Query["TenantId"]))
            {
                reqTennantId = Convert.ToInt32((string) context.Request.Query["TenantId"]);
            }
            else
            {
                var routeContext = new RouteContext(context);
                reqTennantId = routeContext.RouteData.Values["TenantId"] != null
                    ? Convert.ToInt32(routeContext.RouteData.Values["TenantId"])
                    : default(int);
            }

            #endregion

            if (tenantId != reqTennantId && reqTennantId != default(int))
                tenantId = reqTennantId;
            //TODO:从Cookie中获取
            return tenantId;
        }
    }
}