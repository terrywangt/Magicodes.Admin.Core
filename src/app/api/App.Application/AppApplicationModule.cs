using Abp.Modules;
using Magicodes.Admin;
using Magicodes.Admin.Configuration;
using Magicodes.App.Application.Configuration;
using Magicodes.Sms.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Reflection;
using System.Text;
using Magicodes.Pay;
using Magicodes.Sms;

namespace Magicodes.App.Application
{
    /// <summary>
    /// 
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

            if (appConfiguration["Authentication:JwtBearer:IsEnabled"] != null && bool.Parse(appConfiguration["Authentication:JwtBearer:IsEnabled"]))
            {
                ConfigureTokenAuth(appConfiguration);
            }
        }

        private void ConfigureTokenAuth(IConfigurationRoot appConfiguration)
        {
            IocManager.Register<TokenAuthConfiguration>();
            var tokenAuthConfig = IocManager.Resolve<TokenAuthConfiguration>();

            tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appConfiguration["Authentication:JwtBearer:SecurityKey"]));
            tokenAuthConfig.Issuer = appConfiguration["Authentication:JwtBearer:Issuer"];
            tokenAuthConfig.Audience = appConfiguration["Authentication:JwtBearer:Audience"];
            tokenAuthConfig.SigningCredentials = new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
            tokenAuthConfig.Expiration = TimeSpan.FromDays(1);
        }
    }
}