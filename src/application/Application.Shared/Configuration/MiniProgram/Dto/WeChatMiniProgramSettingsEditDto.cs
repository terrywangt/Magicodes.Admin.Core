using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.Admin.Configuration.MiniProgram.Dto
{
    public class WeChatMiniProgramSettingsEditDto
    {
        /// <summary>
        /// 小程序Id
        /// </summary>
        [Required]
        public string AppId { get; set; }
        /// <summary>
        /// 小程序密钥
        /// </summary>
        [Required]
        public string AppSecret { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        [Required]
        public bool IsActive { get; set; }
    }
}
