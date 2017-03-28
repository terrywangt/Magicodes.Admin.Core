using Abp.AutoMapper;
using Abp.Modules;
using Magicodes.Admin;
using Magicodes.WeChat.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Abp.AspNetCore.Configuration;
using Magicodes.WeChat.Core.Authorization;
using Magicodes.WeChat.SDK;
using Abp.Runtime.Session;
using Abp.Configuration;
using Magicodes.WeChat.Configuration;
using Newtonsoft.Json;

namespace Magicodes.WeChat.Application
{
    [DependsOn(
        typeof(WeChatCoreModule),
        typeof(AbpAutoMapperModule),
        typeof(AdminApplicationModule))]
    public class WeChatApplicationModule : AbpModule
    {
        IAbpSession AbpSession { get; set; }
        ISettingManager _settingManager { get; set; }

        public WeChatApplicationModule()
        {
            AbpSession = NullAbpSession.Instance;
        }

        public override void PreInitialize()
        {
            //如果useConventionalHttpVerbs设置为true,  HTTP 方法将根据方法名称的约定来匹配:
            //HttpGet: 方法名以"Get"作为前缀
            //Put: Used if method name starts with 'Put' or 'Update'.
            //Delete: Used if method name starts with 'Delete' or 'Remove'.
            //Post: Used if method name starts with 'Post', 'Create' or 'Insert'.
            //Patch: Used if method name starts with 'Patch'.
            //Otherwise, Post is used as default HTTP verb.
            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(WeChatApplicationModule).Assembly, moduleName: "wechat", useConventionalHttpVerbs: true);
            //添加权限程序
            Configuration.Authorization.Providers.Add<WeChatAuthorizationProvider>();

            //添加自定义 AutoMapper 配置
            //Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);


        }
        public override void PostInitialize()
        {
            base.PostInitialize();
        }
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            #region 公众号SDK配置
            //WeChatSDKBuilder.Create()
            //    //设置Api日志记录器
            //    .WithApiLogger(new NLogLogger("WeChatApiLogger"))
            //    //设置支付日志记录器
            //    .WithPayLogger(new NLogLogger("WeChatPayLogger"))
            //    .Build();

            //注册Key。不再需要各个控制注册
            WeChatFrameworkFuncsManager.Current.Register(WeChatFrameworkFuncTypes.GetKey,
                model =>
                {
                    return AbpSession.TenantId.HasValue ? AbpSession.TenantId.Value : 1;
                });

            //注册获取配置函数：根据Key获取微信配置（加载一次后续将缓存）
            WeChatFrameworkFuncsManager.Current.Register(WeChatFrameworkFuncTypes.Config_GetWeChatConfigByKey,
                model =>
                {
                    var arg = model as WeChatApiCallbackFuncArgInfo;
                    _settingManager = IocManager.Resolve<ISettingManager>();
                    if (_settingManager == null)
                    {
                        throw new Exception("无法获取到设置！");
                    }
                    var tenantId = (int)arg.Data;
                    var settingValue = _settingManager.GetSettingValueForTenant(WeChatSettings.TenantManagement.WeChatApiSettings, tenantId);
                    var appConfig = JsonConvert.DeserializeObject<WeChatApiSetting>(settingValue);
                    if (appConfig == null)
                        throw new Exception("您尚未配置公众号，请配置公众号信息！");
                    return appConfig;
                });
            #endregion
        }
    }
}
