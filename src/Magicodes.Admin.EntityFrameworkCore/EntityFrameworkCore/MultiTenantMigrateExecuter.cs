using System;
using System.Collections.Generic;
using Abp.Data;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Castle.Core.Logging;
using Magicodes.Admin.EntityFrameworkCore;
using Magicodes.Admin.Migrations.Seed;
using Magicodes.Admin.MultiTenancy;

namespace Magicodes.Admin.EntityFrameworkCore
{
    /// <summary>
    /// 数据库迁移
    /// </summary>
    public class MultiTenantMigrateExecuter : ITransientDependency
    {
        public ILogger Logger { get; private set; }
        private readonly AbpZeroDbMigrator _migrator;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IDbPerTenantConnectionStringResolver _connectionStringResolver;

        public MultiTenantMigrateExecuter(
            AbpZeroDbMigrator migrator,
            IRepository<Tenant> tenantRepository,
            ILogger logger,
            IDbPerTenantConnectionStringResolver connectionStringResolver)
        {
            Logger = logger;
            _migrator = migrator;
            _tenantRepository = tenantRepository;
            _connectionStringResolver = connectionStringResolver;
        }

        /// <summary>
        /// 执行迁移
        /// </summary>
        public void Run()
        {
            var hostConnStr = _connectionStringResolver.GetNameOrConnectionString(new ConnectionStringResolveArgs(MultiTenancySides.Host));
            if (hostConnStr.IsNullOrWhiteSpace())
            {
                Logger.Error("没有找到名称为'Default'的连接字符串!");
                return;
            }
            Logger.Info("--------------------------------------------------------");
            Logger.Info("主数据库: " + ConnectionStringHelper.GetConnectionString(hostConnStr));
            Logger.Info("主数据库自动迁移已启动...");

            try
            {
                _migrator.CreateOrMigrateForHost(SeedHelper.SeedHostDb);
            }
            catch (Exception ex)
            {
                Logger.Info("迁移主数据库时发生错误:");
                Logger.Error(ex.Message, ex);
                Logger.Info("迁移已取消!");
                return;
            }

            Logger.Info("主数据库迁移完成.");
            Logger.Info("--------------------------------------------------------");

            var migratedDatabases = new HashSet<string>();
            var tenants = _tenantRepository.GetAllList(t => t.ConnectionString != null && t.ConnectionString != "");
            for (int i = 0; i < tenants.Count; i++)
            {
                var tenant = tenants[i];
                Logger.Info(string.Format("租户数据库迁移已启动... ({0} / {1})", (i + 1), tenants.Count));
                Logger.Info("名称 ： " + tenant.Name);
                Logger.Info("租户名称 ： " + tenant.TenancyName);
                Logger.Info("租户Id ： " + tenant.Id);
                Logger.Info("连接字符串 ： " + SimpleStringCipher.Instance.Decrypt(tenant.ConnectionString));

                if (!migratedDatabases.Contains(tenant.ConnectionString))
                {
                    try
                    {
                        _migrator.CreateOrMigrateForTenant(tenant);
                    }
                    catch (Exception ex)
                    {
                        Logger.Info("迁移租户数据库时发生错误:");
                        Logger.Info(ex.Message, ex);
                        Logger.Info("已跳过当前迁移继续下一个租户数据库迁移...");
                    }

                    migratedDatabases.Add(tenant.ConnectionString);
                }
                else
                {
                    Logger.Info("当前数据库已迁移. 已跳过....");
                }

                Logger.Info(string.Format("租户数据库迁移完成. ({0} / {1})", (i + 1), tenants.Count));
                Logger.Info("--------------------------------------------------------");
            }

            Logger.Info("所有数据库均已完成迁移.");
        }
    }
}