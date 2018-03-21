using System.Reflection;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Session;
using Magicodes.Admin.Authorization;
//using Magicodes.WeChat.SDK.Builder;
using Magicodes.Admin;

namespace Magicodes.App.Application
{
    /// <summary>
    /// 
    /// </summary>
    [DependsOn(
        typeof(AdminCoreModule)
        )]
    public class AppApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}