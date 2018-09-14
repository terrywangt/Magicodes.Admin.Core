using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Abp.Timing;
using Magicodes.Admin.Core.Custom.LogInfos;

namespace Magicodes.App.Application.Users.Cache
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
        public DateTime CreationTime { get; set; }

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
            CreationTime = Clock.Now;
        }
    }
}