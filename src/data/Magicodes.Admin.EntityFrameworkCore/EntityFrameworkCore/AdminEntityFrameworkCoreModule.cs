using Abp;
using Abp.Dependency;
using Abp.Domain.Entities.Auditing;
using Abp.EntityFrameworkCore.Configuration;
using Abp.Extensions;
using Abp.IdentityServer4;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.EntityHistory;
using Magicodes.Admin.Migrations.Seed;
using System;

namespace Magicodes.Admin.EntityFrameworkCore
{
    [DependsOn(
        typeof(AbpZeroCoreEntityFrameworkCoreModule),
        typeof(AdminCoreModule),
        typeof(AbpZeroCoreIdentityServerEntityFrameworkCoreModule)
        )]
    public class AdminEntityFrameworkCoreModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.AbpEfCore().AddDbContext<AdminDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        AdminDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    }
                    else
                    {
                        AdminDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                    }
                });
            }

            //启用实体历史
            Configuration.EntityHistory.Selectors.Add(
                new NamedTypeSelector(
                    "FullAuditedEntities",
                    type => typeof(IFullAudited).IsAssignableFrom(type)
                )
            );
            Configuration.EntityHistory.Selectors.Add("AdminEntities", EntityHistoryHelper.TrackedTypes);
            Configuration.CustomConfigProviders.Add(new EntityHistoryConfigProvider(Configuration));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AdminEntityFrameworkCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            var configurationAccessor = IocManager.Resolve<IAppConfigurationAccessor>();
            using (var scope = IocManager.CreateScope())
            { 
                if (!SkipDbSeed && scope.Resolve<DatabaseCheckHelper>().Exist(configurationAccessor.Configuration["ConnectionStrings:Default"]))
                {
                    //系统启动时自动执行迁移
                    if (Convert.ToBoolean(configurationAccessor.Configuration["Database:AutoMigrate"] ?? "true") && !configurationAccessor.Configuration["ConnectionStrings:Default"].IsNullOrEmpty())
                    {
                        scope.Resolve<MultiTenantMigrateExecuter>().Run();
                    }
                    SeedHelper.SeedHostDb(IocManager);
                }
            }
        }
    }
}
