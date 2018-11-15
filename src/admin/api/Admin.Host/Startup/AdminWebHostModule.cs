using System;
using System.Collections.Generic;
using Abp.AspNetZeroCore;
using Abp.AspNetZeroCore.Web.Authentication.External;
using Abp.AspNetZeroCore.Web.Authentication.External.Facebook;
using Abp.AspNetZeroCore.Web.Authentication.External.Google;
using Abp.AspNetZeroCore.Web.Authentication.External.Microsoft;
using Abp.AspNetZeroCore.Web.Authentication.External.OpenIdConnect;
using Abp.Configuration.Startup;
using Abp.Dependency;
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
using Admin.Application.Custom;
using Magicodes.Admin.Friendships;
using Magicodes.Admin.Chat;
using Magicodes.Admin.Web.Authentication.External;
using Magicodes.Admin.Web.Configuration;
using Magicodes.Unity;
using Magicodes.Sms;
using Magicodes.WeChat.SDK;
using Magicodes.WeChat.SDK.Builder;

namespace Magicodes.Admin.Web.Startup
{
    [DependsOn(
        typeof(AdminApplicationModule),
        typeof(SmsModule),
        typeof(AdminWebCoreModule),
        typeof(AdminAppModule),
        typeof(UnityModule)
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

            //Configuration.Modules.AbpAspNetCore()
            //    .CreateControllersForAppServices(
            //        typeof(AdminApplicationModule).GetAssembly()
            //    );

            //配置后台动态web api
            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(AdminApplicationModule).GetAssembly(), "app"
                );

            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(AdminAppModule).GetAssembly(), "app"
                );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AdminWebHostModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            using (var scope = IocManager.CreateScope())
            {
                if (!scope.Resolve<DatabaseCheckHelper>().Exist(_appConfiguration["ConnectionStrings:Default"]))
                {
                    return;
                }
            }

            if (IocManager.Resolve<IMultiTenancyConfig>().IsEnabled)
            {
                var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
                workManager.Add(IocManager.Resolve<SubscriptionExpirationCheckWorker>());
                workManager.Add(IocManager.Resolve<SubscriptionExpireEmailNotifierWorker>());
            }

            ConfigureExternalAuthProviders();

            ConfigureWeChatSdk();


            IocManager.RegisterIfNot<IChatCommunicator, NullChatCommunicator>();
            //初始化聊天状态监视
            IocManager.Resolve<ChatUserStateWatcher>().Initialize();
        }

        private void ConfigureWeChatSdk()
        {
            WeChatSDKBuilder.Create()
                .WithLoggerAction((tag, message) => { Console.WriteLine(string.Format("Tag:{0}\tMessage:{1}", tag, message)); })
                .Register(WeChatFrameworkFuncTypes.GetKey, model => _appConfiguration["Authentication:WeChat:AppId"])
                .Register(WeChatFrameworkFuncTypes.Config_GetWeChatConfigByKey,
                    model =>
                    {
                        var arg = model as WeChatApiCallbackFuncArgInfo;
                        return new WeChatConfig
                        {
                            AppId = _appConfiguration["Authentication:WeChat:AppId"],
                            AppSecret = _appConfiguration["Authentication:WeChat:AppSecret"]
                        };
                    })
                .Build();
        }

        private void ConfigureExternalAuthProviders()
        {
            var externalAuthConfiguration = IocManager.Resolve<ExternalAuthConfiguration>();

            if (bool.Parse(_appConfiguration["Authentication:WeChat:IsEnabled"]))
            {
                externalAuthConfiguration.Providers.Add(
                    new ExternalLoginProviderInfo(
                        WeChatAuthProviderApi.Name,
                        _appConfiguration["Authentication:WeChat:AppId"],
                        _appConfiguration["Authentication:WeChat:AppSecret"],
                        typeof(WeChatAuthProviderApi)
                    )
                );
            }

            if (bool.Parse(_appConfiguration["Authentication:OpenId:IsEnabled"]))
            {
                externalAuthConfiguration.Providers.Add(
                    new ExternalLoginProviderInfo(
                        OpenIdConnectAuthProviderApi.Name,
                        _appConfiguration["Authentication:OpenId:ClientId"],
                        _appConfiguration["Authentication:OpenId:ClientSecret"],
                        typeof(OpenIdConnectAuthProviderApi),
                        new Dictionary<string, string>
                        {
                            {"Authority", _appConfiguration["Authentication:OpenId:Authority"]},
                            {"LoginUrl",_appConfiguration["Authentication:OpenId:LoginUrl"]}
                        }
                    )
                );
            }

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
        }
    }
}
