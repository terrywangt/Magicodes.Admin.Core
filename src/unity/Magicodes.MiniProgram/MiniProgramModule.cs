using System.Reflection;
using Abp.Modules;
using Magicodes.Admin;
using Magicodes.Admin.Configuration;
using Magicodes.MiniProgram.Startup;

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

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
        public override void PostInitialize()
        {
            var appConfiguration = IocManager.Resolve<IAppConfigurationAccessor>().Configuration;

            MiniProgramStartup.Config(Logger, IocManager,appConfiguration);
            
        }
    }
}
