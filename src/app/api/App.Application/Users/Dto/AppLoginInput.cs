// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : AppLoginInput.cs
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

using System.ComponentModel.DataAnnotations;

namespace Magicodes.App.Application.Users.Dto
{
    /// <summary>
    ///     登陆 输入参数
    /// </summary>
    public class AppLoginInput
    {
        /// <summary>
        ///     手机号码
        /// </summary>
        [Required]
        public string Phone { get; set; }

        /// <summary>
        ///     验证码
        /// </summary>
        [Required]
        public string Code { get; set; }
    }
}