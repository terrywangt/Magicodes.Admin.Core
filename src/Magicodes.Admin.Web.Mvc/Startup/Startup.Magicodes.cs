using Abp.AspNetCore;
using Abp.PlugIns;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Magicodes.Admin.Web.Startup
{
    public partial class Startup
    {
        /// <summary>
        /// 配置自定义Swagger生成
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureCustomServices(IServiceCollection services)
        {
            //设置API文档生成
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Info
                {
                    Title = "API",
                    Version = "v1",
                    Description = "",
                    Contact = new Contact
                    {
                        Name = "心莱科技",
                        Email = "xinlai@xin-lai.com"
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

        private void UsePlugInSources(AbpServiceOptions options)
        {
            //设置插件目录
            var plusPath = Path.Combine(_hostingEnvironment.WebRootPath, "PlugIns");
            if (!Directory.Exists(plusPath))
                Directory.CreateDirectory(plusPath);

            options.PlugInSources.AddFolder(plusPath);
        }
    }
}
