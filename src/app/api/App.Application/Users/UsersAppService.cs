using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Timing;
using Abp.UI;
using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using Magicodes.App.Application.Users.Dto;
using Magicodes.Admin.Authorization.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Abp.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Abp.Authorization.Users;
using Magicodes.Admin.Core.Custom.Authorization;
using Magicodes.Admin.Core.Custom.LogInfos;
using Magicodes.Admin.MultiTenancy;
using Magicodes.App.Application.Configuration;

namespace Magicodes.App.Application.Users
{
    /// <summary>
    /// 用户
    /// </summary>
    [Produces("application/json")]
    [AbpAllowAnonymous]
    [Route("api/[controller]")]
    public class UsersAppService : AppServiceBase, IUsersAppService
    {
        public LogInManager LogInManager { get; set; }
        public TokenAuthConfiguration Configuration { get; set; }

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public IOptions<IdentityOptions> IdentityOptions { get; set; }


        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<AppUserOpenId, long> _appUserOpenIdResposotory;
        private readonly IRepository<SmsCodeLog, long> _smsRepository;
        private readonly UserManager _userManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<Tenant> _tenantRepository;

        public UsersAppService(
            IRepository<User, long> userRepository,
            IRepository<AppUserOpenId, long> appUserOpenIdResposotory,
            IRepository<SmsCodeLog, long> smsRepository,
            UserManager userManager,
            IUnitOfWorkManager unitOfWorkManager,
            IPasswordHasher<User> passwordHasher,
            IRepository<Tenant> tenantRepository)
        {
            this._userRepository = userRepository;
            this._appUserOpenIdResposotory = appUserOpenIdResposotory;
            this._smsRepository = smsRepository;
            this._userManager = userManager;
            _unitOfWorkManager = unitOfWorkManager;
            this._passwordHasher = passwordHasher;
            _tenantRepository = tenantRepository;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        [HttpPost("Register")]
        [UnitOfWork(IsDisabled = true)]
        public async Task<AppRegisterOutput> AppRegister(AppRegisterInput input)
        {
            #region 验证码验证
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var codeValTime = Clock.Now.AddMinutes(-10);
                if (!_smsRepository.GetAll().Any(p => p.Phone == input.Phone && p.SmsCode == input.Code && p.SmsCodeType == SmsCodeTypes.Register && p.CreationTime > codeValTime))
                {
                    throw new UserFriendlyException("短信验证码不正确或已过期!");
                }
                if (_userManager.Users.Any(p => (p.PhoneNumber == input.Phone)))
                {
                    throw new UserFriendlyException("该手机号码已被注册！");
                }
            }
            #endregion
            {
                var from = OpenIdPlatforms.WeChat;
                switch (input.From)
                {
                    case AppRegisterInput.FromEnum.WeChatMiniProgram:
                        from = OpenIdPlatforms.WechatMiniProgram;
                        break;
                    case AppRegisterInput.FromEnum.WeChat:
                        from = OpenIdPlatforms.WeChat;
                        break;
                    default:
                        break;
                }
                return await Register(input.Phone, input.TrueName, input.OpenId, input.UnionId, from);
            }
        }


        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="name"></param>
        /// <param name="openId"></param>
        /// <param name="unionId"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        [UnitOfWork(IsDisabled = true)]
        private async Task<AppRegisterOutput> Register(string phone = null, string name = null, string openId = null, string unionId = null, OpenIdPlatforms platform = OpenIdPlatforms.WeChat)
        {
            var output = new AppRegisterOutput()
            {
            };
            User user = null;
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                bool hasPhone = phone.IsNullOrEmpty() ? false : true;
                //支持游客注册
                var userName = hasPhone ? phone : Guid.NewGuid().ToString("N");
                user = new User
                {
                    IsActive = true,
                    Name = name ?? "游客",
                    PhoneNumber = phone,
                    UserName = userName,
                    TenantId = AbpSession.TenantId,
                    Password = User.CreateRandomPassword(),
                    ShouldChangePasswordOnNextLogin = false,
                    IsPhoneNumberConfirmed = hasPhone,
                    EmailAddress = userName + "@xin-lai.com",
                    Surname = name ?? "游客"
                };
                user.Password = _passwordHasher.HashPassword(user, user.Password);
                output.Phone = phone;

                CheckErrors(await _userManager.CreateAsync(user));

                #region 关联第三方OpenId
                if ((!openId.IsNullOrEmpty()) || (!unionId.IsNullOrEmpty()))
                {
                    var appTokenAuthInput = new AppTokenAuthInput()
                    {
                        OpenIdOrUnionId = unionId ?? openId
                    };
                    switch (platform)
                    {
                        case OpenIdPlatforms.WeChat:
                            appTokenAuthInput.From = openId.IsNullOrWhiteSpace() ? AppTokenAuthInput.FromEnum.WeChatUnionId : AppTokenAuthInput.FromEnum.WeChat;
                            break;
                        case OpenIdPlatforms.WechatMiniProgram:
                            appTokenAuthInput.From = AppTokenAuthInput.FromEnum.WeChatMiniProgram;
                            break;
                        default:
                            break;
                    }
                    _appUserOpenIdResposotory.Insert(new AppUserOpenId()
                    {
                        OpenId = openId,
                        UnionId = unionId,
                        From = platform,
                        UserId = user.Id,
                        CreationTime = Clock.Now,
                        TenantId = AbpSession.TenantId,
                    });
                }
                
                //获取授权信息
                var result = await CreateToken(user);
                output.AccessToken = result.AccessToken;
                output.ExpireInSeconds = result.ExpireInSeconds;
                output.UserId = result.UserId;
                output.Phone = user.PhoneNumber;
                await unitOfWork.CompleteAsync();
                #endregion
            }

           
            return output;
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        [RemoteService(IsEnabled = false)]
        [HttpPost("Login")]
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
        [HttpPost("TokenAuth")]
        [UnitOfWork(isTransactional: false)]
        public async Task<AppTokenAuthOutput> AppTokenAuth(AppTokenAuthInput input)
        {
            AppUserOpenId userOpenIdInfo = null;
            var openIdPlatform = OpenIdPlatforms.WeChat;
            bool isUnionId = false;

            switch (input.From)
            {
                case AppTokenAuthInput.FromEnum.WeChatMiniProgram:
                    {
                        userOpenIdInfo = _appUserOpenIdResposotory.GetAllIncluding(p => p.User).FirstOrDefault(p => p.OpenId == input.OpenIdOrUnionId && p.From == OpenIdPlatforms.WechatMiniProgram);
                        openIdPlatform = OpenIdPlatforms.WechatMiniProgram;
                    }
                    break;
                case AppTokenAuthInput.FromEnum.WeChat:
                    {
                        userOpenIdInfo = _appUserOpenIdResposotory.GetAllIncluding(p => p.User).FirstOrDefault(p => p.OpenId == input.OpenIdOrUnionId && p.From == OpenIdPlatforms.WeChat);
                        openIdPlatform = OpenIdPlatforms.WeChat;
                    }
                    break;
                case AppTokenAuthInput.FromEnum.WeChatUnionId:
                    {
                        userOpenIdInfo = _appUserOpenIdResposotory.GetAllIncluding(p => p.User).FirstOrDefault(p => p.UnionId == input.OpenIdOrUnionId && p.From == OpenIdPlatforms.WeChat);
                        isUnionId = true;
                        openIdPlatform = OpenIdPlatforms.WeChat;
                    }
                    break;
                default:
                    break;
            }
            if (userOpenIdInfo != null)
            {
                return await CreateToken(userOpenIdInfo.User);
            }
            else
            {
                var registerResult = await Register(openId: isUnionId ? null : input.OpenIdOrUnionId, unionId: isUnionId ? input.OpenIdOrUnionId : null, platform: openIdPlatform);

                return new AppTokenAuthOutput()
                {
                    AccessToken = registerResult.AccessToken,
                    ExpireInSeconds = registerResult.ExpireInSeconds,
                    Phone = registerResult.Phone,
                    UserId = registerResult.UserId
                };
            }
        }

        private async Task<AppTokenAuthOutput> CreateToken(User user)
        {
            var result = await CreateUserLoginResult(user);
            var accessToken = CreateAccessToken(CreateJwtClaims(result.Identity));
            var output = new AppTokenAuthOutput()
            {
                AccessToken = accessToken,
                ExpireInSeconds = (int)Configuration.Expiration.TotalSeconds,
                UserId = user.Id,
                Phone = user.PhoneNumber
            };
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
                    tenant = await _tenantRepository.GetAsync(user.TenantId.Value);
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