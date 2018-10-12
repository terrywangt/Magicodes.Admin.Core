// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : AppApplicationModule.cs
//           description :
//   
//           created by 雪雁 at  2018-07-12 18:13
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using Abp.Modules;
using Magicodes.Admin;
using Magicodes.Admin.Configuration;
using Magicodes.App.Application.Configuration;
using Magicodes.Pay;
using Magicodes.Sms;
using Magicodes.Sms.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Reflection;
using System.Text;

namespace Magicodes.App.Application
{
    /// <summary>
    /// </summary>
    [DependsOn(
        typeof(AdminCoreModule),
        typeof(PayModule),
        typeof(SmsModule))]
    public class AppApplicationModule : AbpModule
    {
        public static ISmsService SmsService { get; set; }

        public override void PreInitialize()
        {
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            var appConfiguration = IocManager.Resolve<IAppConfigurationAccessor>().Configuration;

            if (appConfiguration["Authentication:JwtBearer:IsEnabled"] != null &&
                bool.Parse(appConfiguration["Authentication:JwtBearer:IsEnabled"]))
            {
                ConfigureTokenAuth(appConfiguration);
            }
        }

        private void ConfigureTokenAuth(IConfigurationRoot appConfiguration)
        {
            IocManager.Register<TokenAuthConfiguration>();
            var tokenAuthConfig = IocManager.Resolve<TokenAuthConfiguration>();

            tokenAuthConfig.SecurityKey =
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(appConfiguration["Authentication:JwtBearer:SecurityKey"]));
            tokenAuthConfig.Issuer = appConfiguration["Authentication:JwtBearer:Issuer"];
            tokenAuthConfig.Audience = appConfiguration["Authentication:JwtBearer:Audience"];
            tokenAuthConfig.SigningCredentials =
                new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
            //默认为90天
            tokenAuthConfig.Expiration = TimeSpan.FromDays(Convert.ToInt32(appConfiguration["Authentication:JwtBearer:ExpirationDays"] ?? "90"));
        }
    }
}