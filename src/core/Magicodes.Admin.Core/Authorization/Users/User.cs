﻿using System;
using Abp.Authorization.Users;
using Abp.Extensions;
using Abp.Timing;

namespace Magicodes.Admin.Authorization.Users
{
    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    public class User : AbpUser<User>
    {
        public virtual Guid? ProfilePictureId { get; set; }

        public virtual bool ShouldChangePasswordOnNextLogin { get; set; }

        public DateTime? SignInTokenExpireTimeUtc { get; set; }

        public string SignInToken { get; set; }

        public string GoogleAuthenticatorKey { get; set; }

        #region 钱包
        /// <summary>
        /// 余额（以分为单位）
        /// </summary>
        public int Balance { get; set; }

        /// <summary>
        /// 冻结金额（以分为单位）
        /// </summary>
        public int FrozenAmount { get; set; }
        #endregion



        public User()
        {
            IsLockoutEnabled = true;
            IsTwoFactorEnabled = true;
            Balance = 0;
            FrozenAmount = 0;
        }

        /// <summary>
        /// Creates admin <see cref="User"/> for a tenant.
        /// </summary>
        /// <param name="tenantId">Tenant Id</param>
        /// <param name="emailAddress">Email address</param>
        /// <param name="userName">user name(用户名称为了兼容不选租户登陆添加的)</param>
        /// <returns>Created <see cref="User"/> object</returns>
        public static User CreateTenantAdminUser(int tenantId, string emailAddress,string userName=null)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = userName ?? AdminUserName,
                Name = userName ?? AdminUserName,
                Surname = userName ?? AdminUserName,
                EmailAddress = emailAddress
            };

            user.SetNormalizedNames();
            return user;
        }

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public override void SetNewPasswordResetCode()
        {
            /* This reset code is intentionally kept short.
             * It should be short and easy to enter in a mobile application, where user can not click a link.
             */
            PasswordResetCode = Guid.NewGuid().ToString("N").Truncate(10).ToUpperInvariant();
        }

        public void Unlock()
        {
            AccessFailedCount = 0;
            LockoutEndDateUtc = null;
        }

        public void SetSignInToken()
        {
            SignInToken = Guid.NewGuid().ToString();
            SignInTokenExpireTimeUtc = Clock.Now.AddMinutes(1).ToUniversalTime();
        }
    }
}