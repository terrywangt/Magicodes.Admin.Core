using Abp.AutoMapper;
using Magicodes.WeChat.Configuration;
using Magicodes.WeChat.SDK;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeChat.Application.Configuration.Dto
{
    [AutoMapFrom(typeof(WeChatApiSetting))]
    public class WeChatApiSettingEditDto : IWeChatConfig
    {
        /// <summary>
        /// AppId
        /// </summary>
        [MaxLength(50)]
        [Required]
        [Display(Name = "AppId")]
        public string AppId { get; set; }
        /// <summary>
        /// AppSecret
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Display(Name = "AppSecret")]
        [DataType(DataType.Password)]
        public string AppSecret { get; set; }
        /// <summary>
        /// 微信号
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "微信号")]
        public string WeiXinAccount { get; set; }

        /// <summary>
        /// 填写服务器配置时必须，为了安全，请生成自己的Token。注意：正式公众号的Token只允许英文或数字的组合，长度为3-32字符
        /// </summary>
        [MaxLength(200)]
        public string Token { get; set; }

    }
}
