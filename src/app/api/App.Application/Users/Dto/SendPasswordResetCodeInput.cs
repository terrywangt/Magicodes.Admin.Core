// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : SendPasswordResetCodeInput.cs
//           description :
//   
//           created by 雪雁 at  2018-09-13 10:09
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Users;

namespace Magicodes.App.Application.Users.Dto
{
    /// <summary>
    /// </summary>
    public class SendPasswordResetCodeInput
    {
        /// <summary>
        ///     手机验证码
        /// </summary>
        [Required]
        [MaxLength(AbpUserBase.MaxPhoneNumberLength)]
        public string PhoneNumber { get; set; }
    }
}