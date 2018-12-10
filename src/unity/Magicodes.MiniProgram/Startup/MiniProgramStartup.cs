// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : MiniProgramStartup.cs
//           description :
//   
//           created by 雪雁 at  2018-12-10 13:54
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using Magicodes.Admin.Configuration;
using Magicodes.WeChat.MiniProgram.Builder;
using Magicodes.WeChat.MiniProgram.Config;
using Microsoft.Extensions.Configuration;

namespace Magicodes.MiniProgram.Startup
{
    public class MiniProgramStartup
    {
        /// <summary>
        ///     配置微信小程序
        /// </summary>
        public static void Config(ILogger logger, IIocManager iocManager, IConfigurationRoot config, ISettingManager settingManager)
        {
            //日志函数
            void LogAction(string tag, string message)
            {
                if (tag.Equals("error", StringComparison.CurrentCultureIgnoreCase))
                    logger.Error(message);
                else
                    logger.Debug(message);
            }
            var configInfo = new DefaultMiniProgramConfig
            {
                MiniProgramAppId = config["WeChat:MiniProgram:AppId"],
                MiniProgramAppSecret = config["WeChat:MiniProgram:AppSecret"]
            };
            //从用户设置读取配置
            if (Convert.ToBoolean(settingManager.GetSettingValue(AppSettings.WeChatMiniProgram.IsActive)))
            {
                configInfo.MiniProgramAppId = settingManager.GetSettingValue(AppSettings.WeChatMiniProgram.AppId);
                configInfo.MiniProgramAppSecret = settingManager.GetSettingValue(AppSettings.WeChatMiniProgram.AppSecret);
            }
            MiniProgramSDKBuilder.Create()
                //设置日志记录
                .WithLoggerAction(LogAction).RegisterGetKeyFunc(() =>
                {
                    var key = iocManager.Resolve<IAbpSession>()?.TenantId;
                    if (key == null) return "0";

                    return key.ToString();
                }).RegisterGetConfigByKeyFunc(key => configInfo).Build();
        }
    }
}