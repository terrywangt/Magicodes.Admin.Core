using Abp.AspNetZeroCore;
using Abp.AspNetZeroCore.Web.Authentication.External;
using Abp.AspNetZeroCore.Web.Authentication.External.Facebook;
using Abp.AspNetZeroCore.Web.Authentication.External.Google;
using Abp.AspNetZeroCore.Web.Authentication.External.Microsoft;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading.BackgroundWorkers;
using Abp.Timing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.EntityFrameworkCore;
using Magicodes.Admin.MultiTenancy;
using Abp.AspNetCore.Configuration;
using Magicodes.Admin.Friendships;
using Magicodes.Admin.Chat;
using Abp.Dependency;

namespace Magicodes.Admin.Web.Startup
{
    [DependsOn(
        typeof(AdminWebCoreModule)
    )]
    public class AdminWebHostModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public AdminWebHostModule(
            IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.Modules.AbpWebCommon().MultiTenancy.DomainFormat = _appConfiguration["App:ServerRootAddress"] ?? "http://localhost:22742/";
            Configuration.Modules.AspNetZero().LicenseCode = _appConfiguration["AbpZeroLicenseCode"];

            //配置后台动态web api
            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(AdminApplicationModule).GetAssembly(), "app"
                );

            Configuration.EntityHistory.IsEnabled = true;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AdminWebHostModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            if (!DatabaseCheckHelper.Exist(_appConfiguration["ConnectionStrings:Default"]))
            {
                return;
            }

            if (IocManager.Resolve<IMultiTenancyConfig>().IsEnabled)
            {
                var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
                workManager.Add(IocManager.Resolve<SubscriptionExpirationCheckWorker>());
                workManager.Add(IocManager.Resolve<SubscriptionExpireEmailNotifierWorker>());
            }

            ConfigureExternalAuthProviders();

            IocManager.RegisterIfNot<IChatCommunicator, NullChatCommunicator>();
            //初始化聊天状态监视
            IocManager.Resolve<ChatUserStateWatcher>().Initialize();
        }

        private void ConfigureExternalAuthProviders()
        {
            var externalAuthConfiguration = IocManager.Resolve<ExternalAuthConfiguration>();

            if (_appConfiguration["Authentication:Facebook:IsEnabled"] != null && bool.Parse(_appConfiguration["Authentication:Facebook:IsEnabled"]))
            {
                externalAuthConfiguration.Providers.Add(
                    new ExternalLoginProviderInfo(
                        FacebookAuthProviderApi.Name,
                        _appConfiguration["Authentication:Facebook:AppId"],
                        _appConfiguration["Authentication:Facebook:AppSecret"],
                        typeof(FacebookAuthProviderApi)
                    )
                );
            }

            if (_appConfiguration["Authentication:Google:IsEnabled"] != null && bool.Parse(_appConfiguration["Authentication:Google:IsEnabled"]))
            {
                externalAuthConfiguration.Providers.Add(
                    new ExternalLoginProviderInfo(
                        GoogleAuthProviderApi.Name,
                        _appConfiguration["Authentication:Google:ClientId"],
                        _appConfiguration["Authentication:Google:ClientSecret"],
                        typeof(GoogleAuthProviderApi)
                    )
                );
            }

            //not implemented yet. Will be implemented with https://github.com/aspnetzero/aspnet-zero-angular/issues/5
            if (_appConfiguration["Authentication:Microsoft:IsEnabled"] != null && bool.Parse(_appConfiguration["Authentication:Microsoft:IsEnabled"]))
            {
                externalAuthConfiguration.Providers.Add(
                    new ExternalLoginProviderInfo(
                        MicrosoftAuthProviderApi.Name,
                        _appConfiguration["Authentication:Microsoft:ConsumerKey"],
                        _appConfiguration["Authentication:Microsoft:ConsumerSecret"],
                        typeof(MicrosoftAuthProviderApi)
                    )
                );
            }
        }
    }
}
