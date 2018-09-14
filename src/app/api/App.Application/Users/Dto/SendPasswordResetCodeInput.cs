using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Users;

namespace Magicodes.App.Application.Users.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class SendPasswordResetCodeInput
    {
        /// <summary>
        /// 手机验证码
        /// </summary>
        [Required]
        [MaxLength(AbpUserBase.MaxPhoneNumberLength)]
        public string PhoneNumber { get; set; }
    }
}
