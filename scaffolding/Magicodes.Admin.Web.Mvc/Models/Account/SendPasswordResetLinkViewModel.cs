using System.ComponentModel.DataAnnotations;

namespace Magicodes.Admin.Web.Models.Account
{
    public class SendPasswordResetLinkViewModel
    {
        [Required]
        public string EmailAddress { get; set; }
    }
}