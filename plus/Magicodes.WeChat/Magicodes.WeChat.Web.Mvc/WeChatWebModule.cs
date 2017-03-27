using Abp.Configuration;
using Abp.Modules;
using Abp.Resources.Embedded;
using Magicodes.Admin.Configuration;
using Magicodes.Home.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Magicodes.Admin.Configuration.Host;
using Abp.AutoMapper;
using Abp.Modules;
using Magicodes.Admin;
using Magicodes.Admin.Authorization;
using Abp.Authorization;

namespace Magicodes.WeChat.Web.Mvc
{
    [DependsOn(
        typeof(AdminCoreModule),
        typeof(AbpAutoMapperModule),
        typeof(AdminApplicationModule))]
    public class HomeModule : AbpModule
    {

        public override void PreInitialize()
        {
        }

        public override void PostInitialize()
        {
            
        }

        public override void Initialize()
        {
            ////添加导航
            //Configuration.Navigation.Providers.Add<MetronicNavigationProvider>();

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
