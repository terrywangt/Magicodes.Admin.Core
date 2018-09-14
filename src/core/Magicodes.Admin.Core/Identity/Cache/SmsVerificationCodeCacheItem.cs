using System;
using Abp.Timing;

namespace Magicodes.Admin.Identity.Cache
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SmsVerificationCodeCacheItem
    {
        public const string CacheName = "AppSmsVerificationCodeCache";

        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; } = Clock.Now;

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpiredTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SmsVerificationCodeCacheItem()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        public SmsVerificationCodeCacheItem(string code)
        {
            Code = code;
        }
    }
}