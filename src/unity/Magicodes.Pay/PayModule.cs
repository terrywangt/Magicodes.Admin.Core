// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : PayModule.cs
//           description :
//   
//           created by 雪雁 at  2018-08-06 10:21
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System.Reflection;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Magicodes.Admin;
using Magicodes.Admin.Configuration;
using Magicodes.Alipay;
using Magicodes.Pay.Startup;
using Magicodes.Pay.WeChat;
using Magicodes.PayNotify;

namespace Magicodes.Pay
{
    /// <summary>
    /// </summary>
    [DependsOn(
        typeof(AdminCoreModule)
    )]
    public class PayModule : AbpModule
    {
        public override void PreInitialize()
        {

        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
        public override void PostInitialize()
        {
            var settingManager = IocManager.Resolve<ISettingManager>();

            var appConfiguration = IocManager.Resolve<IAppConfigurationAccessor>().Configuration;
            //配置支付
            PayStartup.ConfigAsync(Logger, IocManager, appConfiguration, settingManager).Wait();
            //注册支付回调控制器
            IocManager.Register<PayNotifyController>(DependencyLifeStyle.Transient);
        }
    }
}