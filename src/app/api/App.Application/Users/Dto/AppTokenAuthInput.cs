// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : AppTokenAuthInput.cs
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
    ///     授权访问 输入参数
    /// </summary>
    public class AppTokenAuthInput
    {
        public enum FromEnum
        {
            /// <summary>
            ///     小程序
            /// </summary>
            WeChatMiniProgram = 0,

            /// <summary>
            ///     微信公众号
            /// </summary>
            WeChat = 1,

            /// <summary>
            ///     微信开放平台Id
            /// </summary>
            WeChatUnionId = 2
        }

        /// <summary>
        ///     用户开放Id，比如小程序、公众号的Id
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string OpenIdOrUnionId { get; set; }

        /// <summary>
        ///     来自
        /// </summary>
        public FromEnum From { get; set; }
    }
}