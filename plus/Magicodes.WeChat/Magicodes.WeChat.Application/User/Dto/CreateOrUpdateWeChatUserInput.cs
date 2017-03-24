using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeChat.Application.User.Dto
{
    public class CreateOrUpdateWeChatUserInput
    {
        [Required]
        public WeChatUserEditDto WeChatUser { get; set; }
    }
}
