using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Json;
using Abp.Timing;
using Abp.UI;
using Castle.Core.Logging;
using Magicodes.Admin.Configuration;
using Magicodes.Pay.WeChat;
using Magicodes.Pay.WeChat.Builder;
using Magicodes.PayNotify.Builder;
using Magicodes.Sms.Aliyun;
using Magicodes.Sms.Aliyun.Builder;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Magicodes.App.Application.Startup
{
    public class AppStartup
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

            //微信支付配置
            WeChatPayBuilder.Create()
                //设置日志记录
                .WithLoggerAction(LogAction).RegisterGetPayConfigFunc(() => new WeChatPayConfig()
                {
                    MchId = config["WeChat:Pay:MchId"],
                    PayNotifyUrl = config["WeChat:Pay:NotifyUrl"],
                    TenPayKey = config["WeChat:Pay:TenPayKey"],
                    PayAppId = config["WeChat:Pay:AppId"]
                }).Build();

            //阿里云短信设置
            AliyunSmsBuilder.Create()
                //设置日志记录
                .WithLoggerAction(LogAction).SetSettingsFunc(() => new AliyunSmsSettting(config)).Build();

            AppApplicationModule.SmsService = new AliyunSmsService();

            //支付回调设置
            PayNotifyBuilder
                .Create()
                //设置日志记录
                .WithLoggerAction(LogAction).WithPayNotifyFunc(input =>
                {
                    switch (input.Provider)
                    {
                        case "wechat":
                            {
                                var api = new WeChatPayApi();
                                var output = api.PayNotifyHandler(input.Request.Body);
                                if (output.IsSuccess())
                                {
                                    var resultLog = output.ToJsonString();
                                    logger.Info("微信支付处理成功: " + resultLog);

                                    //获取微信支付自定义数据
                                    if (string.IsNullOrWhiteSpace(output.Attach))
                                        throw new UserFriendlyException("自定义参数不允许为空！");
                                    var data = JsonConvert.DeserializeObject<JObject>(output.Attach);
                                    var key = data["key"].ToString();

                                    switch (key)
                                    {
                                        case "订单支付":
                                            {
                                                var orderId = Convert.ToInt64(data["code"]);
                                                break;
                                            }
                                    }

                                    return Task.FromResult("<xml><return_code><![CDATA[SUCCESS]]></return_code></xml>");
                                }

                                //此处编写失败处理逻辑
                                var failLog = output.ToJsonString();
                                logger.Error("微信支付处理失败: " + failLog);
                                return Task.FromResult(
                                    "<xml><return_code><![CDATA[FAIL]]></return_code><return_msg><![CDATA[回调处理失败]]></return_msg></xml>");
                            }
                        default:
                            break;
                    }

                    return null;
                }).Build();
        }
    }
}