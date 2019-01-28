using Abp.Modules;
using Abp.Reflection.Extensions;
using Magicodes.Admin;
using Magicodes.WeChat.SDK;
using Magicodes.WeChat.SDK.Builder;

namespace Magicodes.WeChat
{
    [DependsOn(
    typeof(AdminCoreModule))]
    public class WeChatModule : AbpModule
    {
        public override void PreInitialize()
        {
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(WeChatModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            //TODO: 伍潜
            //1）公众号配置：参考支付配置
            //2）AccessToken统一封装，支持分布式架构
            //3）封装模板消息服务，便于消息发送

            //WeChatSDKBuilder.Create()
            //    .WithLoggerAction((tag, message) => { Console.WriteLine(string.Format("Tag:{0}\tMessage:{1}", tag, message)); })
            //    .Register(WeChatFrameworkFuncTypes.GetKey, model => _appConfiguration["Authentication:WeChat:AppId"])
            //    .Register(WeChatFrameworkFuncTypes.Config_GetWeChatConfigByKey,
            //        model =>
            //        {
            //            var arg = model as WeChatApiCallbackFuncArgInfo;
            //            return new WeChatConfig
            //            {
            //                AppId = _appConfiguration["Authentication:WeChat:AppId"],
            //                AppSecret = _appConfiguration["Authentication:WeChat:AppSecret"]
            //            };
            //        })
            //    .Build();
        }
    }
}
