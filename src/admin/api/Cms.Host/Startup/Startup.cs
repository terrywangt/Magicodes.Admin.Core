using System;
using System.IO;
using Abp.AspNetCore;
using Abp.Castle.Logging.Log4Net;
using Abp.Domain.Repositories;
using Castle.Facilities.Logging;
using Cms.Host.Route;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.Contents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cms.Host.Startup
{
    public class Startup
    {
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IHostingEnvironment env)
        {
            _hostingEnvironment = env;
            _appConfiguration = env.GetAppConfiguration();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            try
            {
                return services.AddAbp<CmsHostModule>(
                    options =>
                    {
                        //配置日志
                        var log4NetConfig = Path.Combine(_hostingEnvironment.ContentRootPath, "log4net.config");
                        options.IocManager.IocContainer.AddFacility<LoggingFacility>(f =>
                            f.UseAbpLog4Net().WithConfig(log4NetConfig));
                    }
                );
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //初始化 ABP framework.
            app.UseAbp(options =>
            {
                options.UseAbpRequestLocalization = false; //used below: UseAbpRequestLocalization
            });

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
            //app.UseCookiePolicy();
            app.UseMvc(routes =>
            {
                routes.Routes.Add(new RouteProvider(app.ApplicationServices));
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}