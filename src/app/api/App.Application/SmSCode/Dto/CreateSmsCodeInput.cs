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
            ///     注册
            /// </summary>
            Register = 0,

            /// <summary>
            ///     登陆
            /// </summary>
            Login = 1
        }

        /// <summary>
        ///     手机号码
        /// </summary>
        [Required]
        public string Phone { get; set; }

        /// <summary>
        ///     验证码类型
        /// </summary>
        public SmsCodeTypeEnum SmsCodeType { get; set; }
    }
}