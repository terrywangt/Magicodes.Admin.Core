// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : UsersAppService.cs
//           description :
//   
//           created by 雪雁 at  2018-07-30 10:04
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Runtime.Caching;
using Abp.Timing;
using Abp.UI;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Core.Custom.Authorization;
using Magicodes.Admin.Identity;
using Magicodes.Admin.MultiTenancy;
using Magicodes.App.Application.Configuration;
using Magicodes.App.Application.Users.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Magicodes.App.Application.Users
{
    /// <summary>
    ///     用户
    /// </summary>
    [Produces("application/json")]
    [AbpAllowAnonymous]
    [Route("api/[controller]")]
    public class UsersAppService : AppServiceBase, IUsersAppService
    {
        private readonly IRepository<AppUserOpenId, long> _appUserOpenIdResposotory;
        private readonly ICacheManager _cacheManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ISmsSender _smsSender;
        private readonly ISmsVerificationCodeManager _smsVerificationCodeManager;
        private readonly IRepository<Tenant> _tenantRepository;

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly UserManager _userManager;

        private readonly IRepository<User, long> _userRepository;

        public UsersAppService(
            IRepository<User, long> userRepository,
            IRepository<AppUserOpenId, long> appUserOpenIdResposotory,
            UserManager userManager,
            IUnitOfWorkManager unitOfWorkManager,
            IPasswordHasher<User> passwordHasher,
            IRepository<Tenant> tenantRepository,
            ICacheManager cacheManager,
            ISmsVerificationCodeManager smsVerificationCodeManager,
            ISmsSender smsSender)
        {
            _userRepository = userRepository;
            _appUserOpenIdResposotory = appUserOpenIdResposotory;
            _userManager = userManager;
            _unitOfWorkManager = unitOfWorkManager;
            _passwordHasher = passwordHasher;
            _tenantRepository = tenantRepository;
            _cacheManager = cacheManager;
            _smsVerificationCodeManager = smsVerificationCodeManager;
            _smsSender = smsSender;
        }

        public LogInManager LogInManager { get; set; }
        public TokenAuthConfiguration Configuration { get; set; }

        public IOptions<IdentityOptions> IdentityOptions { get; set; }

        /// <summary>
        ///     注册或登陆
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        [HttpPost("")]
        [UnitOfWork(IsDisabled = true)]
        public async Task<AppRegisterOrLoginOutput> AppRegisterOrLogin(AppRegisterOrLoginInput input)
        {
            #region 验证码验证

            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var codeValid =
                    await _smsVerificationCodeManager.VerifyCode(input.Phone, input.Code, "RegisterOrLogin");
                if (!codeValid) throw new UserFriendlyException("短信验证码不正确或已过期!");
            }

            #endregion

            {
                OpenIdPlatforms? from = null;
                switch (input.From)
                {
                    case AppRegisterOrLoginInput.FromEnum.WeChatMiniProgram:
                        from = OpenIdPlatforms.WechatMiniProgram;
                        break;
                    case AppRegisterOrLoginInput.FromEnum.WeChat:
                        from = OpenIdPlatforms.WeChat;
                        break;
                    case null:
                        break;
                    default:
                        break;
                }

                return await RegisterOrLogin(input.Phone, input.TrueName, input.OpenId, input.UnionId, from);
            }
        }

        /// <summary>
        ///     授权访问
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        [HttpPost("TokenAuth")]
        [UnitOfWork(false)]
        public async Task<AppTokenAuthOutput> AppTokenAuth(AppTokenAuthInput input)
        {
            AppUserOpenId userOpenIdInfo = null;
            var openIdPlatform = OpenIdPlatforms.WeChat;
            var isUnionId = false;

            switch (input.From)
            {
                case AppTokenAuthInput.FromEnum.WeChatMiniProgram:
                {
                    userOpenIdInfo = _appUserOpenIdResposotory.GetAllIncluding(p => p.User).FirstOrDefault(p =>
                        p.OpenId == input.OpenIdOrUnionId && p.From == OpenIdPlatforms.WechatMiniProgram);
                    openIdPlatform = OpenIdPlatforms.WechatMiniProgram;
                }
                    break;
                case AppTokenAuthInput.FromEnum.WeChat:
                {
                    userOpenIdInfo = _appUserOpenIdResposotory.GetAllIncluding(p => p.User).FirstOrDefault(p =>
                        p.OpenId == input.OpenIdOrUnionId && p.From == OpenIdPlatforms.WeChat);
                    openIdPlatform = OpenIdPlatforms.WeChat;
                }
                    break;
                case AppTokenAuthInput.FromEnum.WeChatUnionId:
                {
                    userOpenIdInfo = _appUserOpenIdResposotory.GetAllIncluding(p => p.User).FirstOrDefault(p =>
                        p.UnionId == input.OpenIdOrUnionId && p.From == OpenIdPlatforms.WeChat);
                    isUnionId = true;
                    openIdPlatform = OpenIdPlatforms.WeChat;
                }
                    break;
                default:
                    break;
            }

            if (userOpenIdInfo != null) return await CreateToken(userOpenIdInfo.User);

            var registerResult = await RegisterOrLogin(openId: isUnionId ? null : input.OpenIdOrUnionId,
                unionId: isUnionId ? input.OpenIdOrUnionId : null, platform: openIdPlatform);

            return new AppTokenAuthOutput
            {
                AccessToken = registerResult.AccessToken,
                ExpireInSeconds = registerResult.ExpireInSeconds,
                Phone = registerResult.Phone,
                UserId = registerResult.UserId
            };
        }

        /// <summary>
        ///     获取密码重置Code
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        [HttpGet("PasswordResetCode")]
        public async Task SendPasswordResetCode(SendPasswordResetCodeInput input)
        {
            var user = GetUserByChecking(input.PhoneNumber);
            user.SetNewPasswordResetCode();
            //发送短信密码重置验证码
            await _smsSender.SendCodeAsync(user.PhoneNumber, user.PasswordResetCode);
        }

        /// <summary>
        ///     重置密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<ResetPasswordOutput> ResetPassword(ResetPasswordInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.UserId);
            if (user == null || user.PasswordResetCode.IsNullOrEmpty() || user.PasswordResetCode != input.ResetCode)
                throw new UserFriendlyException(L("InvalidPasswordResetCode"), L("InvalidPasswordResetCode_Detail"));

            user.Password = _passwordHasher.HashPassword(user, input.Password);
            user.PasswordResetCode = null;
            user.IsPhoneNumberConfirmed = true;
            user.ShouldChangePasswordOnNextLogin = false;

            await UserManager.UpdateAsync(user);

            var tokenResult = await CreateToken(user);
            return new ResetPasswordOutput
            {
                AccessToken = tokenResult.AccessToken,
                ExpireInSeconds = tokenResult.ExpireInSeconds,
                UserId = tokenResult.UserId,
                PhoneNumber = tokenResult.Phone
            };
        }


        /// <summary>
        ///     注册或登陆
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="name"></param>
        /// <param name="openId"></param>
        /// <param name="unionId"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        [UnitOfWork(IsDisabled = true)]
        private async Task<AppRegisterOrLoginOutput> RegisterOrLogin(string phone = null, string name = null,
            string openId = null, string unionId = null, OpenIdPlatforms? platform = null)
        {
            var output = new AppRegisterOrLoginOutput();
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var user = _userManager.Users.FirstOrDefault(p => p.PhoneNumber == phone);
                if (user != null)
                {
                    await BindAndTokenAuth(openId, unionId, platform, output, user, unitOfWork);
                    return output;
                }

                var hasPhone = !phone.IsNullOrEmpty();
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
                await BindAndTokenAuth(openId, unionId, platform, output, user, unitOfWork);
            }

            return output;
        }

        /// <summary>
        ///     绑定第三方并且授权
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="unionId"></param>
        /// <param name="platform"></param>
        /// <param name="output"></param>
        /// <param name="user"></param>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        private async Task BindAndTokenAuth(string openId, string unionId, OpenIdPlatforms? platform,
            AppRegisterOrLoginOutput output, User user, IUnitOfWorkCompleteHandle unitOfWork)
        {
            #region 关联第三方OpenId

            if (!openId.IsNullOrEmpty() || !unionId.IsNullOrEmpty())
            {
                var appTokenAuthInput = new AppTokenAuthInput
                {
                    OpenIdOrUnionId = unionId ?? openId
                };
                switch (platform)
                {
                    case OpenIdPlatforms.WeChat:
                        appTokenAuthInput.From = openId.IsNullOrWhiteSpace()
                            ? AppTokenAuthInput.FromEnum.WeChatUnionId
                            : AppTokenAuthInput.FromEnum.WeChat;
                        break;
                    case OpenIdPlatforms.WechatMiniProgram:
                        appTokenAuthInput.From = AppTokenAuthInput.FromEnum.WeChatMiniProgram;
                        break;
                    default:
                        break;
                }


                if (platform != null && !_appUserOpenIdResposotory.GetAll().Any(p =>
                        (p.OpenId == openId || p.UnionId == unionId) && p.From == platform && p.UserId == user.Id &&
                        p.TenantId == AbpSession.TenantId))
                {
                    var appUserOpenId = new AppUserOpenId
                    {
                        OpenId = openId,
                        UnionId = unionId,
                        From = platform.Value,
                        UserId = user.Id,
                        CreationTime = Clock.Now,
                        TenantId = AbpSession.TenantId
                    };
                    _appUserOpenIdResposotory.Insert(appUserOpenId);
                }
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


        /// <summary>
        ///     检查用户
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        private User GetUserByChecking(string phoneNumber)
        {
            var user = UserManager.Users.FirstOrDefault(p => p.PhoneNumber == phoneNumber);
            if (user == null) throw new UserFriendlyException(L("InvalidPhoneNumber"));

            if (!user.IsActive) throw new UserFriendlyException(L("UserIsNotActive"));

            return user;
        }

        private async Task<AppTokenAuthOutput> CreateToken(User user)
        {
            var result = await CreateUserLoginResult(user);
            var accessToken = CreateAccessToken(CreateJwtClaims(result.Identity));
            var output = new AppTokenAuthOutput
            {
                AccessToken = accessToken,
                ExpireInSeconds = (int) Configuration.Expiration.TotalSeconds,
                UserId = user.Id,
                Phone = user.PhoneNumber
            };
            return output;
        }

        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                Configuration.Issuer,
                Configuration.Audience,
                claims,
                now,
                now.Add(expiration ?? Configuration.Expiration),
                Configuration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private async Task<AbpLoginResult<Tenant, User>> CreateUserLoginResult(User user, Tenant tenant = null)
        {
            AbpLoginResult<Tenant, User> result;
            //判断是否存在租户
            if (user.TenantId.HasValue)
            {
                if (tenant == null) tenant = await _tenantRepository.GetAsync(user.TenantId.Value);
                //创建登陆
                result = await LogInManager.CreateLoginResultAsync(user, tenant);
            }
            else
            {
                //登陆
                result = await LogInManager.CreateLoginResultAsync(user, null);
            }

            return result;
        }

        private List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == IdentityOptions.Value.ClaimsIdentity.UserIdClaimType);

            if (IdentityOptions.Value.ClaimsIdentity.UserIdClaimType != JwtRegisteredClaimNames.Sub)
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value));

            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            });

            return claims;
        }
    }
}