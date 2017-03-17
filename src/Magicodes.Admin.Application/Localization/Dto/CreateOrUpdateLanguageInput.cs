using System.ComponentModel.DataAnnotations;

namespace Magicodes.Admin.Localization.Dto
{
    public class CreateOrUpdateLanguageInput
    {
        [Required]
        public ApplicationLanguageEditDto Language { get; set; }
    }
}