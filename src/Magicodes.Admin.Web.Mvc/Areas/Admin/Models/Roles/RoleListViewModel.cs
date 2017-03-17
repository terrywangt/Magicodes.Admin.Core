using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Magicodes.Admin.Web.Areas.Admin.Models.Roles
{
    public class RoleListViewModel
    {
        public List<ComboboxItemDto> Permissions { get; set; }
    }
}