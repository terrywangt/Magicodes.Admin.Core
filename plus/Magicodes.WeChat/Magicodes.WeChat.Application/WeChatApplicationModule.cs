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

namespace Magicodes.WeChat.Application
{
    [DependsOn(
        typeof(WeChatCoreModule),
        typeof(AbpAutoMapperModule),
        typeof(AdminApplicationModule))]
    public class WeChatApplicationModule : AbpModule
    {
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

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
