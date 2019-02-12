﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Abp;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Abp.Runtime.Security;
using Abp.Threading;
using Abp.UI;
using Dapper;
using Magicodes.Admin.Authorization.Roles;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.MultiTenancy;
using Magicodes.Admin.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;


namespace Magicodes.Admin.Authorization.Users
{
    /// <summary>
    /// User manager.
    /// Used to implement domain logic for users.
    /// Extends <see cref="AbpUserManager{TRole,TUser}"/>.
    /// </summary>
    public class UserManager : AbpUserManager<Role, User>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ILocalizationManager _localizationManager;

        public UserManager(
            UserStore userStore,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager> logger,
            RoleManager roleManager,
            IPermissionManager permissionManager,
            IUnitOfWorkManager unitOfWorkManager,
            ICacheManager cacheManager,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IOrganizationUnitSettings organizationUnitSettings,
            ISettingManager settingManager,
            ILocalizationManager localizationManager)
            : base(
                  roleManager,
                  userStore,
                  optionsAccessor,
                  passwordHasher,
                  userValidators,
                  passwordValidators,
                  keyNormalizer,
                  errors,
                  services,
                  logger,
                  permissionManager,
                  unitOfWorkManager,
                  cacheManager,
                  organizationUnitRepository,
                  userOrganizationUnitRepository,
                  organizationUnitSettings,
                  settingManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _localizationManager = localizationManager;
        }

        [UnitOfWork]
        public virtual async Task<User> GetUserOrNullAsync(UserIdentifier userIdentifier)
        {
            using (_unitOfWorkManager.Current.SetTenantId(userIdentifier.TenantId))
            {
                return await FindByIdAsync(userIdentifier.UserId.ToString());
            }
        }

        public User GetUserOrNull(UserIdentifier userIdentifier) => AsyncHelper.RunSync(() => GetUserOrNullAsync(userIdentifier));

        public async Task<User> GetUserAsync(UserIdentifier userIdentifier)
        {
            var user = await GetUserOrNullAsync(userIdentifier);
            if (user == null)
            {
                throw new Exception("There is no user: " + userIdentifier);
            }

            return user;
        }

        public User GetUser(UserIdentifier userIdentifier) => AsyncHelper.RunSync(() => GetUserAsync(userIdentifier));

        public override Task<IdentityResult> SetRoles(User user, string[] roleNames)
        {
            if (user.Name == "admin" && !roleNames.Contains(StaticRoleNames.Host.Admin))
            {
                throw new UserFriendlyException(L("AdminRoleCannotRemoveFromAdminUser"));
            }

            return base.SetRoles(user, roleNames);
        }

        public override async Task SetGrantedPermissionsAsync(User user, IEnumerable<Permission> permissions)
        {
            CheckPermissionsToUpdate(user, permissions);

            await base.SetGrantedPermissionsAsync(user, permissions);
        }

        private void CheckPermissionsToUpdate(User user, IEnumerable<Permission> permissions)
        {
            if (user.Name == AbpUserBase.AdminUserName &&
                (!permissions.Any(p => p.Name == AppPermissions.Pages_Administration_Roles_Edit) ||
                !permissions.Any(p => p.Name == AppPermissions.Pages_Administration_Users_ChangePermissions)))
            {
                throw new UserFriendlyException(L("YouCannotRemoveUserRolePermissionsFromAdminUser"));
            }
        }

        private new string L(string name) => _localizationManager.GetString(AdminConsts.LocalizationSourceName, name);

        /// <summary>
        /// 更新余额，可用于充值或扣款
        /// </summary>
        /// <param name="totalFee"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task UpdateRechargeInfo(int totalFee, User user = null)
        {
            if (totalFee == 0)
            {
                throw new UserFriendlyException("金额不能等于0！");
            }

            if (user == null)
            {
                user = await FindByIdAsync(AbpSession.UserId?.ToString());
            }

            if (user == null || user.IsActive == false || user.IsDeleted)
            {
                throw new UserFriendlyException("用户信息异常！");
            }

            if (totalFee < 0 && user.Balance + totalFee < 0)
            {
                throw new UserFriendlyException("余额不足！");
            }
            user.Balance += totalFee * 100;
        }

        /// <summary>
        /// 更新充值金额
        /// </summary>
        /// <param name="userIdentifier"></param>
        /// <param name="totalFee">金额（元）</param>
        /// <returns></returns>
        public async Task UpdateRechargeInfo(UserIdentifier userIdentifier, int totalFee)
        {
            var user = await GetUserOrNullAsync(userIdentifier);
            await UpdateRechargeInfo(totalFee, user);
        }

        /// <summary>
        /// 获取到所用数据库所有用户信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetAllUsers()
        {
            List<User> users = new List<User>();

            List<string> tenantConnectionStrings = new List<string>();

            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder(), addUserSecrets: true);
            
            using (var connection = new MySqlConnection(configuration.GetConnectionString(AdminConsts.ConnectionStringName)))
            {
                await connection.OpenAsync();

                var userSql = "select *from abpusers";
                var userQuery = await connection.QueryAsync<User>(userSql);
                users.AddRange(userQuery);

                var tenantSql = "select *from abptenants";
                var tenantQuery = await connection.QueryAsync<Tenant>(tenantSql);
                foreach (var item in tenantQuery)
                {
                    if (item.ConnectionString != null)
                    {
                        tenantConnectionStrings.Add(SimpleStringCipher.Instance.Decrypt(item.ConnectionString));
                    }
                }

                await connection.CloseAsync();
            }

            if (tenantConnectionStrings.Count > 0)
            {
                foreach (var item in tenantConnectionStrings)
                {
                    using (var connection = new MySqlConnection(item))
                    {
                        await connection.OpenAsync();

                        var userSql = "select *from abpusers";
                        var userQuery = await connection.QueryAsync<User>(userSql);
                        users.AddRange(userQuery);

                        connection.Close();
                    }
                }
            }

            return users;
        }
    }
}