using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.Admin.Configuration.MiniProgram.Dto
{
    public class MiniProgramSettingsEditDto
    {
        /// <summary>
        /// 微信小程序配置
        /// </summary>
        public WeChatMiniProgramSettingsEditDto WeChatMiniProgram { get; set; }
    }
}
