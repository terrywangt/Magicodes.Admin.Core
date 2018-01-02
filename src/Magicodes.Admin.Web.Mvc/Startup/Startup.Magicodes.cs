using Abp;
using Abp.AspNetCore;
using Abp.Hangfire;
using Abp.PlugIns;
using Hangfire;
using Magicodes.Admin.Authorization;
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

            if (bool.Parse(_appConfiguration["SwaggerDoc:IsEnabled"]))
            {
                //设置API文档生成
                services.AddSwaggerGen(options =>
                {
                    options.DescribeAllEnumsAsStrings();
                    options.SwaggerDoc("v1", new Info
                    {
                        Title = _appConfiguration["SwaggerDoc:Title"],
                        Version = _appConfiguration["SwaggerDoc:Version"],
                        Description = _appConfiguration["SwaggerDoc:Description"],
                        Contact = new Contact
                        {
                            Name = _appConfiguration["SwaggerDoc:Contact:Name"],
                            Email = _appConfiguration["SwaggerDoc:Contact:Email"]
                        }
                    });

                    //遍历所有xml并加载
                    var paths = new List<string>();
                    var xmlFiles = new DirectoryInfo(Path.Combine(_hostingEnvironment.WebRootPath, "PlugIns")).GetFiles("*.Application.xml");
                    foreach (var item in xmlFiles)
                    {
                        paths.Add(item.FullName);
                    }
                    var binXmlFiles = new DirectoryInfo(_hostingEnvironment.ContentRootPath).GetFiles("*.Application.xml");
                    foreach (var item in binXmlFiles)
                    {
                        paths.Add(item.FullName);
                    }
                    foreach (var filePath in paths)
                    {
                        options.IncludeXmlComments(filePath);
                    }
                    options.DocInclusionPredicate((docName, description) => true);
                    options.DocumentFilter<HiddenApiFilter>();
                });
            }

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

            if (bool.Parse(_appConfiguration["SwaggerDoc:IsEnabled"]))
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint
                app.UseSwagger();
                // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Admin API V1");
                }); //URL: /swagger
            }

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
