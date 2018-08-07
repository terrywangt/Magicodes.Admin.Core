using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Magicodes.Admin.Web.Authentication.JwtBearer
{
    public class AdminJwtSecurityTokenHandler : ISecurityTokenValidator
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public AdminJwtSecurityTokenHandler()
        {
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public bool CanValidateToken => true;

        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        public bool CanReadToken(string securityToken)
        {
            return _tokenHandler.CanReadToken(securityToken);
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            var cacheManager = IocManager.Instance.Resolve<ICacheManager>();
            var principal = _tokenHandler.ValidateToken(securityToken, validationParameters, out validatedToken);
            var userIdentifier = principal.Claims.First(c => c.Type == AppConsts.UserIdentifier);
            var refreshTokenInClaims = principal.Claims.First(c => c.Type == AppConsts.RefreshTokenName);
            var refreshToken = cacheManager.GetCache(AppConsts.RefreshTokenName).GetOrDefault(userIdentifier.Value + refreshTokenInClaims.Value);

            return refreshToken == null ? throw new SecurityTokenException("invalid") : principal;
        }
    }
}