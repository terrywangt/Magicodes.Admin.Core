// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : AppLoginOutput.cs
//           description :
//   
//           created by 雪雁 at  2018-07-30 10:04
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

namespace Magicodes.App.Application.Users.Dto
{
    /// <summary>
    ///     登陆 输出参数
    /// </summary>
    public class AppLoginOutput
    {
        /// <summary>
        ///     用户Id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///     手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        ///     访问AccessToken
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        ///     过期时间
        /// </summary>
        public int ExpireInSeconds { get; set; }
    }
}