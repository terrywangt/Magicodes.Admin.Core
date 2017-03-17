using System.ComponentModel.DataAnnotations;

namespace Magicodes.Admin.Authorization.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}
