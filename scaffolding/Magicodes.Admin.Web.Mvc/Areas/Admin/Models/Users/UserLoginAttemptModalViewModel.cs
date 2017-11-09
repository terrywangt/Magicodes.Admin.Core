using System.Collections.Generic;
using Magicodes.Admin.Authorization.Users.Dto;

namespace Magicodes.Admin.Web.Areas.Admin.Models.Users
{
    public class UserLoginAttemptModalViewModel
    {
        public List<UserLoginAttemptDto> LoginAttempts { get; set; }
    }
}