using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Magicodes.Admin.Authorization.Users;

namespace Magicodes.Admin.Authorization.OpenId
{
    /// <summary>
    /// 用户OpenId关联表
    /// </summary>
    public class AppUserOpenId : Entity<long>, IMayHaveTenant, IHasCreationTime, IHasModificationTime
    {
        /// <summary>
        /// 开放平台用户唯一标识
        /// </summary>
        [MaxLength(50)]
        public string OpenId { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// 来自（平台）
        /// </summary>
        public OpenIdPlatforms From { get; set; }

        /// <summary>
        /// 开放平台统一Id
        /// </summary>
        [MaxLength(50)]
        public string UnionId { get; set; }
    }
}
