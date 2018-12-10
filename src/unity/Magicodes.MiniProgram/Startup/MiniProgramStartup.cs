using System;
using Abp.Dependency;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using Magicodes.WeChat.MiniProgram.Builder;
using Microsoft.Extensions.Configuration;

namespace Magicodes.MiniProgram.Startup
{
    public class MiniProgramStartup
    {
        /// <summary>
        ///     配置微信小程序
        /// </summary>
        public static void Config(ILogger logger, IIocManager iocManager, IConfigurationRoot config)
        {
            //日志函数
            void LogAction(string tag, string message)
            {
                if (tag.Equals("error", StringComparison.CurrentCultureIgnoreCase))
                    logger.Error(message);
                else
                    logger.Debug(message);
            }


            MiniProgramSDKBuilder.Create()
                //设置日志记录
                .WithLoggerAction(LogAction).RegisterGetKeyFunc(() =>
                {
                    var key = iocManager.Resolve<IAbpSession>()?.TenantId;
                    if (key == null)
                        return "0";
                    return key.ToString();
                }).RegisterGetConfigByKeyFunc(key => new MiniProgramConfig
                {
                    MiniProgramAppId = config["WeChat:MiniProgram:AppId"],
                    MiniProgramAppSecret = config["WeChat:MiniProgram:AppSecret"]
                }).Build();
        }
    }
}