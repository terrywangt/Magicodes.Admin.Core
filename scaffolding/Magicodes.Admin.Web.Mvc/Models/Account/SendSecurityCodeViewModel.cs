using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Magicodes.Admin.Web.Models.Account
{
    public class SendSecurityCodeViewModel
    {
        public List<SelectListItem> Providers { get; set; }

        [Required]
        public string SelectedProvider { get; set; }

        public string ReturnUrl { get; set; }

        public bool RememberMe { get; set; }
    }
}