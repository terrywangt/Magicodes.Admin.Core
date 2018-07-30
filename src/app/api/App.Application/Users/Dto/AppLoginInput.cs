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