using System;
using Abp.Modules;
using Abp.MultiTenancy;
using Abp.TestBase;
using Abp.Zero.Configuration;
using Castle.MicroKernel.Registration;
using Magicodes.Admin.EntityFramework;
using Magicodes.Admin.Tests.Url;
using Magicodes.Admin.Url;
using NSubstitute;

namespace Magicodes.Admin.Tests
{
    [DependsOn(
        typeof(AdminApplicationModule),
        typeof(AdminEntityFrameworkModule),
        typeof(AbpTestBaseModule))]
    public class AdminTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.Timeout = TimeSpan.FromMinutes(30);

            //Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            RegisterFakeService<IAbpZeroDbMigrator>();

            IocManager.Register<IAppUrlService, FakeAppUrlService>();
        }

        private void RegisterFakeService<TService>()
            where TService : class
        {
            IocManager.IocContainer.Register(
                Component.For<TService>()
                    .UsingFactoryMethod(() => Substitute.For<TService>())
                    .LifestyleSingleton()
            );
        }
    }
}
