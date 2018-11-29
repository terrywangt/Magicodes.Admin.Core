using System;
using System.IO;
using System.Linq;
using Abp.AspNetCore;
using Abp.AspNetZeroCore.Web.Authentication.JwtBearer;
using Abp.Castle.Logging.Log4Net;
using Abp.Castle.Logging.NLog;
using Abp.Extensions;
using Castle.Facilities.Logging;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.Identity;
using Magicodes.SwaggerUI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App.Host.Startup
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

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //MVC
            services.AddMvc(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory(DefaultCorsPolicyName));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1); ;


            
            //Configure CORS
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

            //配置JwtBearer验证
            AuthConfigurer.Configure(services, _appConfiguration);


            //添加自定义API文档生成(支持文档配置)
            services.AddCustomSwaggerGen(_appConfiguration, _hostingEnvironment);

            try
            {
                _logger.LogWarning("abp  Begin...");
                //配置ABP以及相关模块依赖
                return services.AddAbp<AppHostModule>(options =>
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
            //初始化 ABP framework.
            app.UseAbp(options =>
            {
                options.UseAbpRequestLocalization = false; //used below: UseAbpRequestLocalization
            });

            //启用跨域资源共享
            app.UseCors(DefaultCorsPolicyName);

            app.UseAuthentication();
            app.UseJwtTokenMiddleware();

            app.UseStaticFiles();

            //if (DatabaseCheckHelper.Exist(_appConfiguration["ConnectionStrings:Default"]))
            //{
            //    app.UseAbpRequestLocalization();
            //}

            //启用自定义API文档(支持文档配置)
            app.UseCustomSwaggerUI(_appConfiguration);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "defaultWithArea",
                    template: "{area}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

            });

            
        }
    }
}
