using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Configuration.SmsCode.Dto
{
    public class AliSmsCodeSettingEditDto
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        [Required]
        public bool IsEnabled { get; set; }
        /// <summary>
        /// accessKeyId
        /// </summary>
        [Required]
        public string AccessKeyId { get; set; }
        /// <summary>
        /// accessKeySecret
        /// </summary>
        [Required]
        public string AccessKeySecret { get; set; }
        /// <summary>
        /// 短信签名-可在短信控制台中找到
        /// </summary>
        [Required]
        public string SignName { get; set; }
        /// <summary>
        /// 短信模板-可在短信控制台中找到，发送国际/港澳台消息时，请使用国际/港澳台短信模版
        /// </summary>
        [Required]
        public string TemplateCode { get; set; }
        /// <summary>
        /// 模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
        /// </summary>
        public string TemplateParam { get; set; }
    }
}
