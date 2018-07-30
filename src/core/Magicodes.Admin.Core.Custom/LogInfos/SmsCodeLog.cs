using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Magicodes.Admin.Core.Custom.LogInfos
{
    /// <summary>
    /// 短信验证码日志
    /// </summary>
    [Description("短信验证码日志")]
    public class SmsCodeLog : Entity<long>, IHasCreationTime
    {
        /// <summary>
        /// 短信验证码
        /// </summary>
        [MaxLength(6)]
        [Required]
        [DisplayName("短信验证码")]
        public string SmsCode { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [MaxLength(15)]
        [DisplayName("手机号码")]
        [Required]
        public string Phone { get; set; }

        /// <summary>
        /// 验证码类型
        /// </summary>
        [DisplayName("验证码类型")]
        [Required]
        public SmsCodeTypes SmsCodeType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
