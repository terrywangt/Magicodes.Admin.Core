using System;
using Abp;
using Abp.Dependency;
using Abp.EntityFrameworkCore.Configuration;
using Abp.Extensions;
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

            //取消该注释将启用组织机构、角色、租户的数据变更历史记录
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
