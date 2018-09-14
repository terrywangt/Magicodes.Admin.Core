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
using System.Threading.Tasks;
using Abp;
using Abp.Configuration;
using Abp.Dependency;
using Abp.UI;
using Castle.Core.Logging;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.Identity;
using Magicodes.Admin.Localization;
using Magicodes.Sms.Aliyun;
using Magicodes.Sms.Aliyun.Builder;
using Magicodes.Sms.Core;

namespace Magicodes.Sms.Services
{
    /// <summary>
    ///     短信发送服务
    /// </summary>
    public class SmsSender : IShouldInitialize, ISingletonDependency, ISmsSender
    {
        private readonly IAppLocalizationManager _appLocalizationManager;
        public SmsSender(IAppLocalizationManager appLocalizationManager)
        {
            _appLocalizationManager = appLocalizationManager;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public IAppConfigurationAccessor AppConfigurationAccessor { get; set; }

        public ISmsService SmsService { get; set; }

        public ISettingManager SettingManager { get; set; }

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
                if (Convert.ToBoolean(SettingManager
                    .GetSettingValueAsync(AppSettings.AliSmsCodeManagement.IsEnabled).Result))
                {
                    //阿里云短信设置
                    AliyunSmsBuilder.Create()
                        //设置日志记录
                        .WithLoggerAction(LogAction)
                        .SetSettingsFunc(() => new AliyunSmsSettting()
                        {
                            AccessKeyId =
                                SettingManager.GetSettingValueAsync(AppSettings.AliSmsCodeManagement.AccessKeyId)
                                    .Result,
                            AccessKeySecret =
                                SettingManager.GetSettingValueAsync(AppSettings.AliSmsCodeManagement.AccessKeySecret)
                                    .Result,
                            SignName = SettingManager.GetSettingValueAsync(AppSettings.AliSmsCodeManagement.SignName)
                                .Result,
                            TemplateCode =
                                SettingManager.GetSettingValueAsync(AppSettings.AliSmsCodeManagement.TemplateCode)
                                    .Result,
                            TemplateParam =
                                SettingManager.GetSettingValueAsync(AppSettings.AliSmsCodeManagement.TemplateParam)
                                    .Result
                        }).Build();
                }
                else
                {
                    //阿里云短信设置
                    AliyunSmsBuilder.Create()
                        //设置日志记录
                        .WithLoggerAction(LogAction)
                        .SetSettingsFunc(() => new AliyunSmsSettting(AppConfigurationAccessor?.Configuration)).Build();
                }
                SmsService = new AliyunSmsService();
            }
            catch (Exception ex)
            {
                Logger.Error("阿里云短信未配置或者配置错误！", ex);
            }
        }

        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        public async Task SendCodeAsync(string phone, string code)
        {
            var result = await SmsService.SendCodeAsync(phone, code);
            if (!result.Success)
            {
                Logger.Error("短信发送失败：" + result.ErrorMessage);
                throw new UserFriendlyException(_appLocalizationManager.L("SmsSendError"));
            }
        }
    }
}