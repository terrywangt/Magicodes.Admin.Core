using Abp.Modules;
using Abp.Zero.EntityFramework;
using Magicodes.Admin;
using Magicodes.Admin.EntityFramework;
using Magicodes.WeChat.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeChat.Data
{
    [DependsOn(
        typeof(AbpZeroEntityFrameworkModule),
        typeof(AdminCoreModule),
        typeof(AdminEntityFrameworkModule),
        typeof(WeChatCoreModule))]
    public class WeChatDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Database.SetInitializer(new CreateDatabaseIfNotExists<WeChatDbContext>());

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<WeChatDbContext, Configuration>());
            //web.config (or app.config for non-web projects) file should contain a connection string named "Default".
            Configuration.DefaultNameOrConnectionString = "Default";
        }
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }

    public class Configuration : DbMigrationsConfiguration<WeChatDbContext>
    {
        public Configuration()
        {
            ContextKey = "Magicodes.WeChat.WeChatDbContext";
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(WeChatDbContext context)
        {

        }
    }
}
