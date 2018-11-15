using Abp.AspNetCore;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.AspNetZeroCore.Web.Authentication.JwtBearer;
using Abp.Castle.Logging.Log4Net;
using Abp.Dependency;
using Abp.Extensions;
using Castle.Facilities.Logging;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.EntityFrameworkCore;
using Magicodes.Admin.Identity;
using Magicodes.Admin.Web.Chat.SignalR;
using Magicodes.Admin.Web.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using System;
using System.Linq;
using Abp.Castle.Logging.NLog;
using Microsoft.Extensions.Logging;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace Magicodes.Admin.Web.Startup
{
    public partial class Startup
    {
        private readonly ILogger _logger;
        private const string DefaultCorsPolicyName = "localhost";

        private readonly IConfigurationRoot _appConfiguration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IHostingEnvironment env, ILogger<Startup> logger)
        {
            _hostingEnvironment = env;
            _appConfiguration = env.GetAppConfiguration();
            _logger = logger;
            _logger.LogInformation($"运行环境:{env.EnvironmentName}");
        }

        /// <summary>
        /// 配置自定义服务
        /// </summary>
        /// <param name="services"></param>
        partial void ConfigureCustomServices(IServiceCollection services);

        partial void CustomConfigure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory);

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //MVC
            services.AddMvc(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory(DefaultCorsPolicyName));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1); ;

            services.AddSignalR(options => { options.EnableDetailedErrors = true; });

            //Configure CORS for angular2 UI
            services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName, builder =>
                {
                    //App:CorsOrigins in appsettings.json can contain more than one address with splitted by comma.
                    builder
                        .WithOrigins(
                            // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                            _appConfiguration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

            ConfigureCustomServices(services);

            ////Recaptcha
            //services.AddRecaptcha(new RecaptchaOptions
            //{
            //    SiteKey = _appConfiguration["Recaptcha:SiteKey"],
            //    SecretKey = _appConfiguration["Recaptcha:SecretKey"]
            //});

            //Configure Abp and Dependency Injection
            return services.AddAbp<AdminWebHostModule>(options =>
            {
                options.IocManager.Register<IAppConfigurationAccessor, AppConfigurationAccessor>(DependencyLifeStyle
                    .Singleton);

                //Configure Log4Net logging
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

                //默认不启动插件目录（不推荐插件模式）
                //options.PlugInSources.AddFolder(Path.Combine(_hostingEnvironment.WebRootPath, "Plugins"), SearchOption.AllDirectories);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //Initializes ABP framework.
            app.UseAbp(options =>
            {
                options.UseAbpRequestLocalization = false; //used below: UseAbpRequestLocalization
            });

            app.UseCors(DefaultCorsPolicyName); //Enable CORS!

            app.UseAuthentication();
            app.UseJwtTokenMiddleware();

            if (bool.Parse(_appConfiguration["IdentityServer:IsEnabled"]))
            {
                app.UseJwtTokenMiddleware("IdentityBearer");
                app.UseIdentityServer();
            }

            app.UseStaticFiles();

            using (var scope = app.ApplicationServices.CreateScope())
            {
                if (scope.ServiceProvider.GetService<DatabaseCheckHelper>().Exist(_appConfiguration["ConnectionStrings:Default"]))
                {
                    app.UseAbpRequestLocalization();
                }
            }

            app.UseSignalR(routes =>
            {
                routes.MapHub<AbpCommonHub>("/signalr");
                routes.MapHub<ChatHub>("/signalr-chat");
            });

            CustomConfigure(app, env, loggerFactory);

        }
    }
}
