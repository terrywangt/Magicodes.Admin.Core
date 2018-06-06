using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.App
{
    /// <summary>
    /// 基础模型
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class EntityBase<TKey> :
        Entity<TKey>,
        IFullAudited,
        IMayHaveTenant
    {
        /// <summary>
        /// 创建者UserId
        /// </summary>
        [Display(Name ="创建者UserId")]
        public long? CreatorUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name ="创建时间")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 最后修改者UserId
        /// </summary>
        [Display(Name ="最后修改者UserId")]
        public long? LastModifierUserId { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [Display(Name ="最后修改时间")]
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 删除者UserId
        /// </summary>
        [Display(Name ="删除者UserId")]
        public long? DeleterUserId { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        [Display(Name ="删除时间")]
        public DateTime? DeletionTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [Display(Name ="是否删除")]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Display(Name ="租户Id")]
        public int? TenantId { get; set; }
    }
}
