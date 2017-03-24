using Abp.AutoMapper;
using Abp.Modules;
using Abp.Zero;
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
            WeChatLocalizationConfigurer.Configure(Configuration.Localization);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

    }
}
