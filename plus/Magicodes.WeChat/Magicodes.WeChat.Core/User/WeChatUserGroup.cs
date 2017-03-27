using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeChat.Core.User
{
    [Table("WeChatUserGroup")]
    public class WeChatUserGroup : Entity<int>, IHasCreationTime, IMayHaveTenant
    {
        public const int MaxNameLength = 30;
        public WeChatUserGroup()
        {
            CreationTime = Clock.Now;
        }
        /// <summary>
        /// 分组id，由微信分配
        /// </summary>
        public int GroupId { get; set; }
        /// <summary>
        /// 分组名字，UTF8编码
        /// </summary>
        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; }
        /// <summary>
        /// 分组内用户数量
        /// </summary>
        public int UsersCount { get; set; }
        public int? TenantId { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
