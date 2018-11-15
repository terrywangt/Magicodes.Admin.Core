using Abp.Extensions;
using Abp.Hangfire;
using Hangfire;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Web.Controllers;
using Magicodes.Admin.Web.IdentityServer;
using Magicodes.Storage.Local.Core;
using Magicodes.SwaggerUI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Magicodes.Admin.Web.Startup
{
    public partial class Startup
    {
        /// <summary>
        /// 配置自定义服务
        /// </summary>
        /// <param name="services"></param>
        partial void ConfigureCustomServices(IServiceCollection services)
        {
            _logger.LogInformation($"IdentityServer:IsEnabled:{_appConfiguration["IdentityServer:IsEnabled"]}");
            //Identity server
            if (bool.Parse(_appConfiguration["IdentityServer:IsEnabled"]))
            {
                IdentityServerRegistrar.Register(services, _appConfiguration);
            }

            //添加自定义API文档生成(支持文档配置)
            services.AddCustomSwaggerGen(_appConfiguration, _hostingEnvironment);
            _logger.LogInformation($"Abp:Hangfire:IsEnabled:{_appConfiguration["Abp:Hangfire:IsEnabled"]}");
            //仅在后台服务启用
            if (!_appConfiguration["Abp:Hangfire:IsEnabled"].IsNullOrEmpty() && Convert.ToBoolean(_appConfiguration["Abp:Hangfire:IsEnabled"]))
            {
                //使用Hangfire替代默认的任务调度
                services.AddHangfire(config =>
                {
                    config.UseSqlServerStorage(_appConfiguration.GetConnectionString("Default"));
                });
            }
        }

        partial void CustomConfigure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //仅在后台服务启用
            if (!_appConfiguration["Abp:Hangfire:IsEnabled"].IsNullOrEmpty() && Convert.ToBoolean(_appConfiguration["Abp:Hangfire:IsEnabled"]) && !_appConfiguration["Abp:Hangfire:DashboardEnabled"].IsNullOrEmpty() && Convert.ToBoolean(_appConfiguration["Abp:Hangfire:DashboardEnabled"]))
            {
                //启用Hangfire仪表盘
                app.UseHangfireDashboard("/hangfire", new DashboardOptions
                {
                    Authorization = new[] { new AbpHangfireAuthorizationFilter(AppPermissions.Pages_Administration_HangfireDashboard) }
                });
                app.UseHangfireServer();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "defaultWithArea",
                    template: "{area}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //启用自定义API文档(支持文档配置)
            app.UseCustomSwaggerUI(_appConfiguration);
        }
    }
}
