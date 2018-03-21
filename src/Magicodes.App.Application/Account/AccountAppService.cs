using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Timing;
using Abp.UI;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.MultiTenancy;
using Magicodes.Admin.Web.Authentication.JwtBearer;
using Magicodes.App.Application.Account.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Magicodes.App.Application.Account
{
    /// <summary>
    /// 账户相关
    /// </summary>
    public class AccountAppService : AppServiceBase, IAccountAppService
    {
        public LogInManager LogInManager { get; set; }
        public TokenAuthConfiguration Configuration { get; set; }

        public IOptions<IdentityOptions> IdentityOptions { get; set; }

        private readonly IRepository<User, long> _userRepository;
        //private readonly IRepository<AppUserOpenId, long> _appUserOpenIdResposotory;
        //private readonly IRepository<SmsCodeLog, long> _smsRepository;
        private readonly UserManager _userManager;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountAppService(
            IRepository<User, long> userRepository,
            //IRepository<AppUserOpenId, long> appUserOpenIdResposotory,
            //IRepository<SmsCodeLog, long> smsRepository,
            UserManager userManager,
            IPasswordHasher<User> passwordHasher)
        {
            this._userRepository = userRepository;
            //this._appUserOpenIdResposotory = appUserOpenIdResposotory;
            //this._smsRepository = smsRepository;
            this._userManager = userManager;
            this._passwordHasher = passwordHasher;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        public async Task<AppRegisterOutput> AppRegister(AppRegisterInput input)
        {
            int? tenantId = null;
            using (AbpSession.Use(tenantId, null))
            {
                using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
                {
                    #region 验证码验证
                    var codeValTime = Clock.Now.AddMinutes(-10);
                    //if (!_smsRepository.GetAll().Any(p => p.Phone == input.Phone && p.SmsCode == input.Code && p.SmsCodeType == SmsCodeTypes.Register && p.CreationTime > codeValTime))
                    //{
                    //    throw new UserFriendlyException("短信验证码不正确或已过期!");
                    //}
                    #endregion
                    {
                        if (_userManager.Users.Any(p => (p.PhoneNumber == input.Phone) && p.TenantId == tenantId))
                        {
                            throw new UserFriendlyException("该手机号码已被注册！");
                        }
                        var user = new User
                        {
                            IsActive = true,
                            Name = input.TrueName,
                            PhoneNumber = input.Phone,
                            UserName = input.Phone,
                            TenantId = AbpSession.TenantId,
                            Password = User.CreateRandomPassword(),
                            ShouldChangePasswordOnNextLogin = false,
                            IsPhoneNumberConfirmed = true,
                            EmailAddress = input.Phone + "@xin-lai.com",
                            Surname = input.TrueName
                        };
                        user.Password = _passwordHasher.HashPassword(user, user.Password);


                        CheckErrors(await _userManager.CreateAsync(user));
                        await CurrentUnitOfWork.SaveChangesAsync();
                        var output = new AppRegisterOutput()
                        {
                            Phone = user.PhoneNumber
                        };
                        //if (!input.OpenId.IsNullOrEmpty() || !input.UnionId.IsNullOrEmpty())
                        //{
                        //    var form = OpenIdPlatforms.WeChat;
                        //    var appTokenAuthInput = new AppTokenAuthInput()
                        //    {
                        //        OpenIdOrUnionId = input.OpenId ?? input.UnionId
                        //    };
                        //    switch (input.From)
                        //    {
                        //        case AppRegisterInput.FromEnum.WeChatMiniProgram:
                        //            form = OpenIdPlatforms.WechatMiniProgram;
                        //            appTokenAuthInput.From = AppTokenAuthInput.FromEnum.WeChatMiniProgram;
                        //            break;
                        //        case AppRegisterInput.FromEnum.WeChat:
                        //            form = OpenIdPlatforms.WeChat;
                        //            appTokenAuthInput.From = input.OpenId.IsNullOrWhiteSpace() ? AppTokenAuthInput.FromEnum.WeChatUnionId : AppTokenAuthInput.FromEnum.WeChat;
                        //            break;
                        //        default:
                        //            break;
                        //    }
                        //    _appUserOpenIdResposotory.Insert(new AppUserOpenId()
                        //    {
                        //        OpenId = input.OpenId,
                        //        UnionId = input.UnionId,
                        //        From = form
                        //    });

                        //    //根据OPENID或者UnionId获取授权信息
                        //    var result = await AppTokenAuth(appTokenAuthInput);

                        //    output.AccessToken = result.AccessToken;
                        //    output.ExpireInSeconds = result.ExpireInSeconds;
                        //    output.UserId = result.UserId;
                        //    output.Phone = user.PhoneNumber;
                        //}
                        return output;
                    }
                }
            }
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        public async Task<AppLoginOutput> AppLogin(AppLoginInput input)
        {
            //TODO:[API]登陆
            //请结合描述或要点实现方法，并且在完成后删除掉TODO注释
            throw new NotSupportedException("AppLogin");
        }

        /// <summary>
        /// 授权访问
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        public async Task<AppTokenAuthOutput> AppTokenAuth(AppTokenAuthInput input)
        {
            var output = new AppTokenAuthOutput()
            {

            };
            //AppUserOpenId userOpenIdInfo = null;
            //using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            //{
            //    switch (input.From)
            //    {
            //        case AppTokenAuthInput.FromEnum.WeChatMiniProgram:
            //            {
            //                userOpenIdInfo = _appUserOpenIdResposotory.GetAllIncluding(p => p.User).FirstOrDefault(p => p.OpenId == input.OpenIdOrUnionId && p.From == OpenIdPlatforms.WechatMiniProgram);
            //            }
            //            break;
            //        case AppTokenAuthInput.FromEnum.WeChat:
            //            {
            //                userOpenIdInfo = _appUserOpenIdResposotory.GetAllIncluding(p => p.User).FirstOrDefault(p => p.OpenId == input.OpenIdOrUnionId && p.From == OpenIdPlatforms.WeChat);
            //            }
            //            break;
            //        case AppTokenAuthInput.FromEnum.WeChatUnionId:
            //            {
            //                userOpenIdInfo = _appUserOpenIdResposotory.GetAllIncluding(p => p.User).FirstOrDefault(p => p.UnionId == input.OpenIdOrUnionId && p.From == OpenIdPlatforms.WeChat);
            //            }
            //            break;
            //        default:
            //            break;
            //    }

            //    if (userOpenIdInfo != null)
            //    {
            //        var result = await CreateUserLoginResult(userOpenIdInfo.User);
            //        var accessToken = CreateAccessToken(CreateJwtClaims(result.Identity));
            //        output.AccessToken = accessToken;
            //        output.ExpireInSeconds = (int)Configuration.Expiration.TotalSeconds;
            //        output.UserId = userOpenIdInfo.User.Id;
            //        output.Phone = userOpenIdInfo.User.PhoneNumber;
            //    }
            //}
            return output;
        }

        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: Configuration.Issuer,
                audience: Configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration ?? Configuration.Expiration),
                signingCredentials: Configuration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private async Task<AbpLoginResult<Tenant, User>> CreateUserLoginResult(User user, Tenant tenant = null)
        {
            AbpLoginResult<Tenant, User> result;
            //判断是否存在租户
            if (user.TenantId.HasValue)
            {
                if (tenant == null)
                {
                    var tenantManager = IocManager.Instance.Resolve<TenantManager>();
                    tenant = await tenantManager.FindByIdAsync(user.TenantId.Value);
                }
                //创建登陆
                result = await LogInManager.CreateLoginResultAsync(user, tenant);
            }
            else
                //登陆
                result = await LogInManager.CreateLoginResultAsync(user, null);
            return result;
        }

        private List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == IdentityOptions.Value.ClaimsIdentity.UserIdClaimType);

            if (IdentityOptions.Value.ClaimsIdentity.UserIdClaimType != JwtRegisteredClaimNames.Sub)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value));
            }

            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }
    }
}