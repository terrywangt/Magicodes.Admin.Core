// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : SmsVerificationCodeCacheExtensions.cs
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

using Abp.Runtime.Caching;

namespace Magicodes.App.Application.Users.Cache
{
    public static class SmsVerificationCodeCacheExtensions
    {
        public static ITypedCache<string, SmsVerificationCodeCacheItem> GetSmsVerificationCodeCache(
            this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, SmsVerificationCodeCacheItem>(SmsVerificationCodeCacheItem.CacheName);
        }
    }
}