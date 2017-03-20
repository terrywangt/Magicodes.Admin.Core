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

namespace Magicodes.Home
{
    [DependsOn(
        typeof(AdminCoreModule),
        typeof(AbpAutoMapperModule),
        typeof(AdminApplicationModule))]
    public class HomeModule : AbpModule
    {

        public ISettingManager _SettingManager { get; set; }

        public override void PreInitialize()
        {
            //设置语言资源
            HomeLocalizationConfigurer.Configure(Configuration.Localization);
        }

        public override void PostInitialize()
        {
            _SettingManager = IocManager.Resolve<ISettingManager>();
            if (_SettingManager == null)
            {
                return;
            }
            //修改默认首页路径
            _SettingManager.ChangeSettingForApplication(AppSettings.TenantManagement.DefaultUrl, "/web/");
        }

        public override void Initialize()
        {
            ////添加导航
            //Configuration.Navigation.Providers.Add<MetronicNavigationProvider>();

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
