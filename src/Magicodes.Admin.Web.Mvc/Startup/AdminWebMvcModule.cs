using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading.BackgroundWorkers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.MultiTenancy;
using Magicodes.Admin.Web.Areas.Admin.Startup;
using Magicodes.Admin.Web.Authentication.External;
using Magicodes.Admin.Web.Authentication.External.Facebook;
using Magicodes.Admin.Web.Authentication.External.Google;
using Magicodes.Admin.Web.Authentication.External.Microsoft;

namespace Magicodes.Admin.Web.Startup
{
    [DependsOn(
        typeof(AdminWebCoreModule)
    )]
    public class AdminWebMvcModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public AdminWebMvcModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.Modules.AbpWebCommon().MultiTenancy.DomainFormat = _appConfiguration["App:WebSiteRootAddress"] ?? "http://localhost:62114/";

            Configuration.Navigation.Providers.Add<AdminNavigationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AdminWebMvcModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            if (!IocManager.Resolve<IMultiTenancyConfig>().IsEnabled)
            {
                return;
            }
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            workManager.Add(IocManager.Resolve<SubscriptionExpirationCheckWorker>());
            workManager.Add(IocManager.Resolve<SubscriptionExpireEmailNotifierWorker>());
            ConfigureExternalAuth();
        }

        private void ConfigureExternalAuth()
        {
            var externalAuthConfiguration = IocManager.Resolve<ExternalAuthConfiguration>();

            if (bool.Parse(_appConfiguration["Authentication:Facebook:IsEnabled"]))
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

            if (bool.Parse(_appConfiguration["Authentication:Google:IsEnabled"]))
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
            if (bool.Parse(_appConfiguration["Authentication:Microsoft:IsEnabled"]))
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

            if (bool.Parse(_appConfiguration["Authentication:QQ:IsEnabled"]))
            {
                externalAuthConfiguration.Providers.Add(
                    new ExternalLoginProviderInfo(
                        MicrosoftAuthProviderApi.Name,
                        _appConfiguration["Authentication:QQ:ClientId"],
                        _appConfiguration["Authentication:QQ:ClientSecret"],
                        typeof(MicrosoftAuthProviderApi)
                    )
                );
            }

            if (bool.Parse(_appConfiguration["Authentication:Weibo:IsEnabled"]))
            {
                externalAuthConfiguration.Providers.Add(
                    new ExternalLoginProviderInfo(
                        MicrosoftAuthProviderApi.Name,
                        _appConfiguration["Authentication:Weibo:ClientId"],
                        _appConfiguration["Authentication:Weibo:ClientSecret"],
                        typeof(MicrosoftAuthProviderApi)
                    )
                );
            }

            if (bool.Parse(_appConfiguration["Authentication:Weixin:IsEnabled"]))
            {
                externalAuthConfiguration.Providers.Add(
                    new ExternalLoginProviderInfo(
                        MicrosoftAuthProviderApi.Name,
                        _appConfiguration["Authentication:Weixin:ClientId"],
                        _appConfiguration["Authentication:Weixin:ClientSecret"],
                        typeof(MicrosoftAuthProviderApi)
                    )
                );
            }

            if (bool.Parse(_appConfiguration["Authentication:Baidu:IsEnabled"]))
            {
                externalAuthConfiguration.Providers.Add(
                    new ExternalLoginProviderInfo(
                        MicrosoftAuthProviderApi.Name,
                        _appConfiguration["Authentication:Baidu:ClientId"],
                        _appConfiguration["Authentication:Baidu:ClientSecret"],
                        typeof(MicrosoftAuthProviderApi)
                    )
                );
            }
        }

    }
}