using System;
using Abp.Json;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Magicodes.AppSession
{
    public class AppSession : IAppSession
    {
        private const string AppSessionCookieName = "magicodes.appUserId_{0}";

        public AppSession()
        {
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }
        public string AppUserId { get; set; }
        public string AppType { get; set; }
        public IAppSessionManager AppSessionManager { get; set; }
        public IAppTenantManager AppTenantManager { get; set; }

        public IHttpContextAccessor HttpContextAccessor { get; set; }

        public IAppSessionInfo AppSessionInfo { get; set; }

        public T GetUserInfo<T>() where T : class
        {
            if (string.IsNullOrEmpty(AppSessionInfo?.AppUserId))
                throw new ArgumentNullException(nameof(AppSessionInfo));
            var func = AppSessionManager.GetUserInfoFunc(AppSessionInfo.RequestType);
            return func?.Invoke(AppSessionInfo) as T;
        }

        public T GetUserExtendInfo<T>() where T : class
        {
            if (string.IsNullOrEmpty(AppSessionInfo?.AppUserId))
                throw new ArgumentNullException(nameof(AppSessionInfo));
            var func = AppSessionManager.GetUserInfoExtendFunc(AppSessionInfo.RequestType);
            return func?.Invoke(AppSessionInfo) as T;
        }

        public void SetAppSession(IAppSessionInfo appSessionInfo)
        {
            Logger.Debug("SetAppSession___Start");
            var tenantId = AppTenantManager.GetTenantId();
            var cookieName = string.Format(AppSessionCookieName, tenantId);
            var context = HttpContextAccessor.HttpContext;
            AppSessionInfo = appSessionInfo ?? throw new ArgumentNullException(nameof(appSessionInfo));
            context.Response.Cookies.Append(cookieName, AppSessionInfo.ToJsonString());
            context.Items["AppSessionInfo"] = appSessionInfo;
        }

        public void Initialize()
        {
            Logger.Debug("AppSession___Initialize");
            var context = HttpContextAccessor.HttpContext;
            if (context == null)
            {
                Logger.Warn("HttpContextAccessor.HttpContext Is Null!");
                throw new ArgumentNullException(nameof(context));
            }
            if (AppSessionManager == null)
            {
                Logger.Warn("AppSessionManager Is Null!");
                throw new ArgumentNullException(nameof(AppSessionManager));
            }
            if (AppTenantManager == null)
            {
                Logger.Warn("AppTenantManager Is Null!");
                throw new ArgumentNullException(nameof(AppTenantManager));
            }
            if (context.Items["AppSessionInfo"] != null)
            {
                AppSessionInfo = context.Items["AppSessionInfo"] as AppSessionInfo;
            }
            else
            {
                if (context.Items["AppSessionInfo"] != null)
                {
                    AppSessionInfo = context.Items["AppSessionInfo"] as IAppSessionInfo;
                    Logger.Debug("AppSession___Initialize   context.Items____End");
                    return;
                }
                var tenantId = AppTenantManager.GetTenantId();
                var cookieName = string.Format(AppSessionCookieName, tenantId);
                //从Cookie中获取OpenId
                var cookie = context.Request.Cookies[cookieName];
                if (string.IsNullOrEmpty(cookie))
                {
                    Logger.Debug("AppSession___Initialize  cookie Is Null____End");
                    return;
                }
                AppSessionInfo = JsonConvert.DeserializeObject<AppSessionInfo>(cookie);
            }
            Logger.Debug("AppSession___Initialize Deserialize from cookie____End");
        }
    }
}