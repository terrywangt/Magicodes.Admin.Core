using Abp;
using Abp.AspNetCore;
using Abp.Hangfire;
using Abp.PlugIns;
using Hangfire;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Web.SwaggerUI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Magicodes.Admin.Web.Startup
{
    public partial class Startup
    {
        /// <summary>
        /// 配置自定义
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureCustomServices(IServiceCollection services)
        {
            if (bool.Parse(_appConfiguration["App:Gzip:IsEnabled"]))
            {
                //启用GZIP压缩
                services.AddResponseCompression(options =>
                {
                    options.Providers.Add<GzipCompressionProvider>();
                    var mimeTypes = _appConfiguration["App:Gzip:MimeTypes"];
                    if (!string.IsNullOrWhiteSpace(mimeTypes))
                    {
                        options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(mimeTypes.Split(','));
                    }
                });

                services.Configure<GzipCompressionProviderOptions>(options =>
                {
                    options.Level = CompressionLevel.Fastest;
                });
            }

            //添加自定义API文档生成(支持文档配置)
            services.AddCustomSwaggerGen(_appConfiguration, _hostingEnvironment);

            //启用Hangfire (使用Hangfire 代替默认的 job manager)
            services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(_appConfiguration.GetConnectionString("Default"));
            });
        }

        private void ConfigureCustom(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (bool.Parse(_appConfiguration["App:Gzip:IsEnabled"]))
            {
                //使用GZIP压缩
                app.UseResponseCompression();
            }

            //启用自定义API文档(支持文档配置)
            app.UseCustomSwaggerUI(_appConfiguration);

            //启用Hangfire后台面板
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new AbpHangfireAuthorizationFilter(AppPermissions.Pages_Administration_HangfireDashboard) }
            });
            app.UseHangfireServer();
        }

        /// <summary>
        /// 启用插件目录
        /// </summary>
        /// <param name="options"></param>
        private void UsePlugInSources(AbpBootstrapperOptions options)
        {
            //设置插件目录
            var plusPath = Path.Combine(_hostingEnvironment.WebRootPath, "PlugIns");
            if (!Directory.Exists(plusPath))
                Directory.CreateDirectory(plusPath);

            options.PlugInSources.AddFolder(plusPath);
        }
    }
}
