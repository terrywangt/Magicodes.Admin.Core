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