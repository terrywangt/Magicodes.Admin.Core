using System.ComponentModel.DataAnnotations;

namespace Magicodes.Admin.Authorization.Accounts.Dto
{
    public class SendEmailActivationLinkInput
    {
        [Required]
        public string EmailAddress { get; set; }
    }
}