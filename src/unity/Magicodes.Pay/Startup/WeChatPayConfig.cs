// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : WeChatPayConfig.cs
//           description :
//   
//           created by 雪雁 at  2018-08-06 10:20
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using Magicodes.Pay.WeChat.Config;

namespace Magicodes.Pay.Startup
{
    public class WeChatPayConfig : IWeChatPayConfig
    {
        public string PayAppId { get; set; }
        public string MchId { get; set; }
        public string PayNotifyUrl { get; set; }
        public string TenPayKey { get; set; }
    }
}