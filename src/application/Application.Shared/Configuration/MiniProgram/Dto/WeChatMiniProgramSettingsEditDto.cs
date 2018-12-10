using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.Admin.Configuration.MiniProgram.Dto
{
    public class WeChatMiniProgramSettingsEditDto
    {
        [Required]
        public string AppId { get; set; }
        [Required]
        public string AppSecret { get; set; }
    }
}
