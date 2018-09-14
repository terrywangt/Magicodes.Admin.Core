// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : VerifySmsCodeInputDto.cs
//           description :
//   
//           created by 雪雁 at  2018-09-13 15:03
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
    public class VerifySmsCodeInputDto
    {
        public enum SmsCodeTypeEnum
        {
            /// <summary>
            ///     注册
            /// </summary>
            Register = 0,

            /// <summary>
            ///     登陆
            /// </summary>
            Login = 1
        }

        /// <summary>
        ///     验证码类型
        /// </summary>
        public SmsCodeTypeEnum SmsCodeType { get; set; }

        /// <summary>
        ///     手机号码
        /// </summary>
        [Required]
        public string PhoneNumber { get; set; }

        /// <summary>
        ///     验证码
        /// </summary>
        [Required]
        public string Code { get; set; }
    }
}