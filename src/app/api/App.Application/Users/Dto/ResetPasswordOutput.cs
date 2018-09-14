// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : ResetPasswordOutput.cs
//           description :
//   
//           created by 雪雁 at  2018-09-13 11:01
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

namespace Magicodes.App.Application.Users.Dto
{
    public class ResetPasswordOutput
    {
        /// <summary>
        ///     访问Token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        ///     过期时间
        /// </summary>
        public int ExpireInSeconds { get; set; }

        /// <summary>
        ///     用户Id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///     手机号码
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}