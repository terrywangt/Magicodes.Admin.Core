using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Security;

namespace Magicodes.Admin.Web.Areas.Admin.Models.Users
{
    public class UsersViewModel
    {
        public string FilterText { get; set; }

        public List<ComboboxItemDto> Permissions { get; set; }

        public List<ComboboxItemDto> Roles { get; set; }
    }
}