using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Configuration.Pay.Dto
{
    public class WeChatPaySettingEditDto
    {
        public string AppId { get; set; }

        public string MchId { get; set; }

        public string PayNotifyUrl { get; set; }

        public string TenPayKey { get; set; }

        public bool IsActive { get; set; }
    }
}
