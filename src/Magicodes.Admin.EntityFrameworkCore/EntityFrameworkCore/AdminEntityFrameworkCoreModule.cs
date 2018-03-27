using Abp;
using Abp.EntityFrameworkCore.Configuration;
using Abp.IdentityServer4;
using Abp.Modules;
using Abp.Organizations;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using Magicodes.Admin.Authorization.Roles;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.Migrations.Seed;
using Magicodes.Admin.MultiTenancy;

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

            //Uncomment below line to write change logs for the entities below:
            //Configuration.EntityHistory.Selectors.Add("AdminEntities", typeof(OrganizationUnit), typeof(Role), typeof(Tenant));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AdminEntityFrameworkCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            var configurationAccessor = IocManager.Resolve<IAppConfigurationAccessor>();
            if (!SkipDbSeed && DatabaseCheckHelper.Exist(configurationAccessor.Configuration["ConnectionStrings:Default"]))
            {
                //系统启动时自动执行迁移
                if (Convert.ToBoolean(configurationAccessor.Configuration["Database:AutoMigrate"] ?? "true") && !configurationAccessor.Configuration["ConnectionStrings:Default"].IsNullOrEmpty())
                {
                    using (var migrateExecuter = IocManager.ResolveAsDisposable<MultiTenantMigrateExecuter>())
                    {
                        migrateExecuter.Object.Run();
                    }
                }
                SeedHelper.SeedHostDb(IocManager);
            }
        }
    }
}
