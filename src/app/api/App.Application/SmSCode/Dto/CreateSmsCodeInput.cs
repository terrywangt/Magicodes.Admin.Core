// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : CreateSmsCodeInput.cs
//           description :
//   
//           created by 雪雁 at  2018-07-30 10:43
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System.ComponentModel.DataAnnotations;

namespace Magicodes.App.Application.SmSCode.Dto
{
    /// <summary>
    ///     请求发送短信验证码 输入参数
    /// </summary>
    public class CreateSmsCodeInput
    {
        public enum SmsCodeTypeEnum
        {
            /// <summary>
            ///     注册或登陆
            /// </summary>
            RegisterOrLogin = 0
        }

        /// <summary>
        ///     手机号码
        /// </summary>
        [Required]
        public string PhoneNumber { get; set; }

        /// <summary>
        ///     验证码类型
        /// </summary>
        public SmsCodeTypeEnum SmsCodeType { get; set; }
    }
}