using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.Admin.Web.SwaggerUI
{
    public static class SwaggerConfigHelper
    {
        /// <summary>
        /// 添加自定义API文档生成(支持文档配置)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="hostingEnvironment"></param>
        public static void AddCustomSwaggerGen(this IServiceCollection services, IConfigurationRoot configuration, IHostingEnvironment hostingEnvironment)
        {
            if (configuration == null || configuration["SwaggerDoc:IsEnabled"] == null)
            {
                return;
            }
            if (bool.Parse(configuration["SwaggerDoc:IsEnabled"]))
            {
                //设置API文档生成
                services.AddSwaggerGen(options =>
                {
                    options.DescribeAllEnumsAsStrings();
                    options.SwaggerDoc("v1", new Info
                    {
                        Title = configuration["SwaggerDoc:Title"],
                        Version = configuration["SwaggerDoc:Version"],
                        Description = configuration["SwaggerDoc:Description"],
                        Contact = new Contact
                        {
                            Name = configuration["SwaggerDoc:Contact:Name"],
                            Email = configuration["SwaggerDoc:Contact:Email"]
                        }
                    });

                    //遍历所有xml并加载
                    var paths = new List<string>();
                    var plusPath = Path.Combine(hostingEnvironment.WebRootPath, "PlugIns");
                    if (Directory.Exists(plusPath))
                    {
                        var xmlFiles = new DirectoryInfo(plusPath).GetFiles("*.Application.xml");
                        foreach (var item in xmlFiles)
                        {
                            paths.Add(item.FullName);
                        }
                    }
                    var binXmlFiles = new DirectoryInfo(hostingEnvironment.ContentRootPath).GetFiles("*.Application.xml", hostingEnvironment.EnvironmentName == "Development" ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                    foreach (var item in binXmlFiles)
                    {
                        paths.Add(item.FullName);
                    }
                    foreach (var filePath in paths)
                    {
                        options.IncludeXmlComments(filePath);
                    }
                    options.DocInclusionPredicate((docName, description) => true);
                    options.DocumentFilter<HiddenApiFilter>(configuration);

                    if (configuration["SwaggerDoc:UseFullNameForSchemaId"] != null && bool.Parse(configuration["SwaggerDoc:UseFullNameForSchemaId"]))
                    {
                        //使用全名作为架构id
                        options.CustomSchemaIds(p => p.FullName);
                    }
                });
            }
        }

        /// <summary>
        /// 启用自定义API文档(支持文档配置)
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        public static void UseCustomSwaggerUI(this IApplicationBuilder app, IConfigurationRoot configuration)
        {
            if (configuration == null || configuration["SwaggerDoc:IsEnabled"] == null)
            {
                return;
            }
            if (bool.Parse(configuration["SwaggerDoc:IsEnabled"]))
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint
                app.UseSwagger();
                // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", configuration["SwaggerDoc:Title"] ?? "App API V1");
                }); //URL: /swagger
            }
        }
    }
}
