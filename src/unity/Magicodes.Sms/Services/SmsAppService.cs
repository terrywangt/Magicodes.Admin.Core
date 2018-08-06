// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : SmsAppService.cs
//           description :
//   
//           created by 雪雁 at  2018-08-06 11:08
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using Abp;
using Abp.Dependency;
using Castle.Core.Logging;
using Magicodes.Admin.Configuration;
using Magicodes.Sms.Aliyun;
using Magicodes.Sms.Aliyun.Builder;
using Magicodes.Sms.Core;

namespace Magicodes.Sms.Services
{
    /// <summary>
    ///     短信发送服务
    /// </summary>
    public class SmsAppService : IShouldInitialize, ISingletonDependency, ISmsAppService
    {
        public SmsAppService()
        {
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public IAppConfigurationAccessor AppConfigurationAccessor { get; set; }

        public ISmsService SmsService { get; set; }

        public void Initialize()
        {
            //日志函数
            void LogAction(string tag, string message)
            {
                if (tag.Equals("error", StringComparison.CurrentCultureIgnoreCase))
                    Logger.Error(message);
                else
                    Logger.Debug(message);
            }

            try
            {
                //阿里云短信设置
                AliyunSmsBuilder.Create()
                    //设置日志记录
                    .WithLoggerAction(LogAction)
                    .SetSettingsFunc(() => new AliyunSmsSettting(AppConfigurationAccessor?.Configuration)).Build();
                SmsService = new AliyunSmsService();
            }
            catch (Exception ex)
            {
                Logger.Error("阿里云短信未配置或者配置错误！", ex);
            }
        }
    }
}