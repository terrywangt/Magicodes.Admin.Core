// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : MiniProgramModule.cs
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

using Abp.Configuration;
using Abp.Modules;
using Magicodes.Admin;
using Magicodes.Admin.Configuration;
using Magicodes.MiniProgram.Startup;
using System.Reflection;

namespace Magicodes.MiniProgram
{
    /// <summary>
    /// </summary>
    [DependsOn(
        typeof(AdminCoreModule)
    )]
    public class MiniProgramModule : AbpModule
    {
        public override void PreInitialize()
        {
        }

        public override void Initialize() => IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

        public override void PostInitialize()
        {
            var appConfiguration = IocManager.Resolve<IAppConfigurationAccessor>().Configuration;
            var settingManager = IocManager.Resolve<ISettingManager>();

            MiniProgramStartup.Config(Logger, IocManager, appConfiguration, settingManager);
        }
    }
}