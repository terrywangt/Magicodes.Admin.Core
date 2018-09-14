// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : SmsVerificationCodeCacheItem.cs
//           description :
//   
//           created by 雪雁 at  2018-09-14 10:09
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using Abp.Timing;

namespace Magicodes.App.Application.Users.Cache
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class SmsVerificationCodeCacheItem
    {
        public const string CacheName = "AppSmsVerificationCodeCache";

        /// <summary>
        /// </summary>
        public SmsVerificationCodeCacheItem()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="code"></param>
        public SmsVerificationCodeCacheItem(string code)
        {
            Code = code;
            CreationTime = Clock.Now;
        }

        /// <summary>
        ///     验证码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}