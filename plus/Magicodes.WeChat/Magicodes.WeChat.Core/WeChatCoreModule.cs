using Abp.AutoMapper;
using Abp.Modules;
using Abp.Zero;
using Magicodes.Admin.Configuration;
using Magicodes.WeChat.Configuration;
using Magicodes.WeChat.Core.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeChat.Core
{
    [DependsOn(
       typeof(AbpZeroCoreModule),
       typeof(AbpAutoMapperModule))]
    public class WeChatCoreModule: AbpModule
    {
        public override void PreInitialize()
        {
            //配置多语言
            WeChatLocalizationConfigurer.Configure(Configuration.Localization);
            //配置公众号设置相关
            Configuration.Settings.Providers.Add<WeChatSettingProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

    }
}
