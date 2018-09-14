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
        /// 验证码
        /// </summary>
        [Required]
        public string Code { get; set; }
    }
}
