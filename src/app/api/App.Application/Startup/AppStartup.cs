using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Json;
using Abp.Timing;
using Abp.UI;
using Castle.Core.Logging;
using Magicodes.Admin.Configuration;
using Magicodes.Alipay;
using Magicodes.Alipay.Builder;
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

            #region 支付配置
            #region 微信支付
            //微信支付配置
            WeChatPayBuilder.Create()
                //设置日志记录
                .WithLoggerAction(LogAction)
                .RegisterGetPayConfigFunc(() => new WeChatPayConfig()
                {
                    MchId = config["WeChat:Pay:MchId"],
                    PayNotifyUrl = config["WeChat:Pay:NotifyUrl"],
                    TenPayKey = config["WeChat:Pay:TenPayKey"],
                    PayAppId = config["WeChat:Pay:AppId"]
                }).Build();
            #endregion

            #region 支付宝支付
            AlipayBuilder.Create()
                .WithLoggerAction(LogAction)
                .RegisterGetPayConfigFunc(() =>
                {
                    var settings = new AlipaySettings
                    {
                        AlipayPublicKey = config["Alipay:PublicKey"],
                        AppId = config["Alipay:AppId"],
                        Uid = config["Alipay:Uid"],
                        PrivateKey = config["Alipay:PrivateKey"]
                    };

                    if (!config["Alipay:CharSet"].IsNullOrWhiteSpace())
                        settings.CharSet = config["Alipay:CharSet"];

                    if (!config["Alipay:Gatewayurl"].IsNullOrWhiteSpace())
                        settings.Gatewayurl = config["Alipay:Gatewayurl"];

                    if (!config["Alipay:Notify"].IsNullOrWhiteSpace())
                        settings.Notify = config["Alipay:Notify"];

                    if (!config["Alipay:SignType"].IsNullOrWhiteSpace())
                        settings.SignType = config["Alipay:SignType"];
                    return settings;
                }).Build();
            #endregion

            #region 支付回调配置
            void PayAction(string code)
            {
                //校验返回的订单金额是否与商户侧的订单金额一致
                //重复处理判断
                //TODO:支付逻辑
            }

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
                                
                                //TODO:签名校验
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
                                                PayAction(orderId.ToString());
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
                        case "alipay":
                            {
                                //TODO:签名校验
                                var ordercode = input.Request.Form["out_trade_no"];
                                var charset = input.Request.Form["charset"];
                                PayAction(ordercode);
                                return Task.FromResult("success");
                            }
                        default:
                            break;
                    }

                    return null;
                }).Build();
            #endregion
            #endregion

            //阿里云短信设置
            AliyunSmsBuilder.Create()
                //设置日志记录
                .WithLoggerAction(LogAction).SetSettingsFunc(() => new AliyunSmsSettting(config)).Build();

            AppApplicationModule.SmsService = new AliyunSmsService();


        }
    }
}