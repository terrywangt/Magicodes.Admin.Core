using System;
using System.IO;
using System.Linq;
using Abp.AspNetCore;
using Abp.AspNetZeroCore.Web.Authentication.JwtBearer;
using Abp.Castle.Logging.Log4Net;
using Abp.Castle.Logging.NLog;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Castle.Facilities.Logging;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.Contents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cms.Host.Startup
{
    public class Startup
    {
        private const string DefaultCorsPolicyName = "localhost";

        private readonly IConfigurationRoot _appConfiguration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger _logger;
        public Startup(IHostingEnvironment env, ILogger<Startup> logger)
        {
            _appConfiguration = env.GetAppConfiguration();
            _hostingEnvironment = env;
            _logger = logger;
            _logger.LogInformation($"EnvironmentName:{env.EnvironmentName}");
            _logger.LogInformation($"ContentRootPath:{env.ContentRootPath}");
            _logger.LogInformation($"WebRootPath:{env.WebRootPath}");
            _logger.LogInformation($"CurrentDirectory:{Directory.GetCurrentDirectory()}");
            _logger.LogWarning($"ConnectionString:{_appConfiguration["ConnectionStrings:Default"]}");
        }

        //public IServiceProvider Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            //services.AddScoped<IColumnInfoAppService, ColumnInfoAppService>();
            //services.BuildServiceProvider();

            services.AddMvc(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory(DefaultCorsPolicyName));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            try
            {
                _logger.LogWarning("abp  Begin...");
                //配置ABP以及相关模块依赖
                return services.AddAbp<CmsHostModule>(options =>
                {
                    //配置日志
                    options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                        f =>
                        {
                            var logType = _appConfiguration["Abp:LogType"];
                            _logger.LogInformation($"LogType:{logType}");
                            if (logType != null && logType == "NLog")
                            {
                                f.UseAbpNLog().WithConfig("nlog.config");
                            }
                            else
                            {
                                f.UseAbpLog4Net().WithConfig("log4net.config");
                            }
                        });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("配置Abp出现错误", ex);
                return null;
            }

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.Routes.Add(new RouteProvider(app.ApplicationServices));
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}
