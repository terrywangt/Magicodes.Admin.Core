using Abp.AspNetCore;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.AspNetZeroCore.Web.Authentication.JwtBearer;
using Abp.Castle.Logging.Log4Net;
using Abp.Castle.Logging.NLog;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Abp.Json;
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
            //输出配置
            _logger.LogDebug(_appConfiguration.GetChildren().ToList().ToJsonString());
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

            var sbuilder = services.AddSignalR(options => { options.EnableDetailedErrors = true; });

            if (!_appConfiguration["Abp:SignalRRedisCache:ConnectionString"].IsNullOrWhiteSpace())
            {
                _logger.LogWarning("Abp:SignalRRedisCache:ConnectionString:" + _appConfiguration["Abp:SignalRRedisCache:ConnectionString"]);
                sbuilder.AddRedis(_appConfiguration["Abp:SignalRRedisCache:ConnectionString"]);
            }

            //Configure CORS for APP
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

            if (bool.Parse(_appConfiguration["App:HttpsRedirection"] ?? "false"))
            {
                //建议开启，以在浏览器显示安全图标
                //设置https重定向端口
                services.AddHttpsRedirection(options =>
                {
                    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                    options.HttpsPort = 443;
                });
            }

            //是否启用HTTP严格传输安全协议(HSTS)
            if (bool.Parse(_appConfiguration["App:UseHsts"] ?? "false"))
            {
                //services.AddHsts(options =>
                //{
                //    options.Preload = true;
                //    options.IncludeSubDomains = true;
                //    options.MaxAge = TimeSpan.FromDays(60);
                //    options.ExcludedHosts.Add("example.com");
                //});
            }

            try
            {
                _logger.LogWarning("ConfigureCustomServices  Begin...");
                ConfigureCustomServices(services);
                _logger.LogWarning("ConfigureCustomServices  End...");
            }
            catch (Exception ex)
            {
                _logger.LogError("执行ConfigureCustomServices出现错误", ex);
            }

            try
            {
                _logger.LogWarning("abp  Begin...");
                //配置ABP以及相关模块依赖
                return services.AddAbp<AdminWebHostModule>(options =>
                {
                    options.IocManager.Register<IAppConfigurationAccessor, AppConfigurationAccessor>(DependencyLifeStyle
                        .Singleton);

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

                    //默认不启动插件目录（不推荐插件模式）
                    //options.PlugInSources.AddFolder(Path.Combine(_hostingEnvironment.WebRootPath, "Plugins"), SearchOption.AllDirectories);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("配置Abp出现错误", ex);
                return null;
            }

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

            //app.UseWebSockets();
            app.UseSignalR(routes =>
            {
                routes.MapHub<AbpCommonHub>("/signalr");
                routes.MapHub<ChatHub>("/signalr-chat");
                ////使用长轮询
                //routes.MapHub<AbpCommonHub>("/signalr", otp => otp.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling);
                //routes.MapHub<ChatHub>("/signalr-chat", otp => otp.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling);
            });

            if (bool.Parse(_appConfiguration["App:HttpsRedirection"] ?? "false"))
            {
                _logger.LogWarning("准备启用HTTS跳转...");
                //建议开启，以在浏览器显示安全图标
                app.UseHttpsRedirection();
            }

            //是否启用HTTP严格传输安全协议(HSTS)【开发环境关闭】
            if (!env.IsDevelopment() && bool.Parse(_appConfiguration["App:UseHsts"] ?? "false"))
            {
                _logger.LogWarning("准备启用HSTS...");
                try
                {
                    app.UseHsts();
                    _logger.LogWarning("成功启用HSTS...");
                }
                catch (Exception ex)
                {
                    _logger.LogError("启用HSTS出现错误", ex);
                }
            }

            try
            {
                _logger.LogWarning("应用自定义配置...");
                CustomConfigure(app, env, loggerFactory);
            }
            catch (Exception ex)
            {
                _logger.LogError("应用自定义配置出现错误", ex);
            }

        }
    }
}
