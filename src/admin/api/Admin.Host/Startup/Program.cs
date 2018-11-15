using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Magicodes.Admin.Web.Startup
{
    public class Program
    {
        public static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) => new WebHostBuilder()
                .UseKestrel((context, opt) =>
                {
                    opt.AddServerHeader = false;
                    //从配置文件读取配置
                    opt.Configure(context.Configuration.GetSection("Kestrel"));
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                            optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
                .UseIISIntegration()
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                })
                .UseStartup<Startup>();
    }
}
