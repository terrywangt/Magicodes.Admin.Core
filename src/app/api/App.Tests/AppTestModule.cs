using Abp.AspNetZeroCore;
using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Modules;
using Abp.Net.Mail;
using Abp.TestBase;
using Abp.Zero.Configuration;
using App.Tests.Configuration;
using App.Tests.DependencyInjection;
using App.Tests.Logging;
using App.Tests.Url;
using Castle.Core.Logging;
using Castle.MicroKernel.Registration;
using Magicodes.Admin;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.Core.Custom;
using Magicodes.Admin.EntityFrameworkCore;
using Magicodes.Admin.Identity;
using Magicodes.Admin.Url;
using Magicodes.App.Application;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System;
using System.IO;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;

namespace App.Tests
{
    [DependsOn(
        typeof(AppCoreModule),
        typeof(AppApplicationModule),
        typeof(AdminCoreModule),
        typeof(AdminEntityFrameworkCoreModule),
        typeof(AbpTestBaseModule))]
    public class AppTestModule : AbpModule
    {
        public AppTestModule(AdminEntityFrameworkCoreModule abpZeroTemplateEntityFrameworkCoreModule) => abpZeroTemplateEntityFrameworkCoreModule.SkipDbContextRegistration = true;

        public override void PreInitialize()
        {
            var configuration = GetConfiguration();

            Configuration.UnitOfWork.Timeout = TimeSpan.FromMinutes(30);
            Configuration.UnitOfWork.IsTransactional = false;

            //Disable static mapper usage since it breaks unit tests (see https://github.com/aspnetboilerplate/aspnetboilerplate/issues/2052)
            Configuration.Modules.AbpAutoMapper().UseStaticMapper = false;

            //Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            RegisterFakeService<AbpZeroDbMigrator>();

            IocManager.Register<ILogger, TestLogger>();

            IocManager.Register<IWebUrlService, FakeWebUrlService>();

            Configuration.ReplaceService<ISmsSender, NullSmsSender>();

            Configuration.ReplaceService<IAppConfigurationAccessor, TestAppConfigurationAccessor>();
            Configuration.ReplaceService<IEmailSender, NullEmailSender>(DependencyLifeStyle.Transient);

            //添加语言源，用于单元测试
            var appPath = Path.Combine(Directory.GetCurrentDirectory(), "Localization", "App");
            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    AdminConsts.AppLocalizationSourceName,
                    new XmlFileLocalizationDictionaryProvider(
                        appPath
                    )
                )
            );

            Configuration.Modules.AspNetZero().LicenseCode = configuration["AbpZeroLicenseCode"];
        }

        public override void Initialize() => ServiceCollectionRegistrar.Register(IocManager);

        private void RegisterFakeService<TService>()
            where TService : class => IocManager.IocContainer.Register(
                Component.For<TService>()
                    .UsingFactoryMethod(() => Substitute.For<TService>())
                    .LifestyleSingleton()
            );

        private static IConfigurationRoot GetConfiguration() => AppConfigurations.Get(Directory.GetCurrentDirectory(), addUserSecrets: true);
    }
}
