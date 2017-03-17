using System.ComponentModel.DataAnnotations;
using Magicodes.Admin.Authorization.Users;

namespace Magicodes.Admin.Configuration.Host.Dto
{
    public class SendTestEmailInput
    {
        [Required]
        [MaxLength(User.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }
    }
}