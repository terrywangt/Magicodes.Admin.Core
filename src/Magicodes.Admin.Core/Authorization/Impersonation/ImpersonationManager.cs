using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Abp.Runtime.Caching;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.UI;
using Magicodes.Admin.Authorization.Users;

namespace Magicodes.Admin.Authorization.Impersonation
{
    public class ImpersonationManager : AdminDomainServiceBase, IImpersonationManager
    {
        public IAbpSession AbpSession { get; set; }

        private readonly ICacheManager _cacheManager;
        private readonly UserManager _userManager;

        public ImpersonationManager(ICacheManager cacheManager, UserManager userManager)
        {
            _cacheManager = cacheManager;
            _userManager = userManager;

            AbpSession = NullAbpSession.Instance;
        }

        public async Task<UserAndIdentity> GetImpersonatedUserAndIdentity(string impersonationToken, string authenticationType)
        {
            var cacheItem = await _cacheManager.GetImpersonationCache().GetOrDefaultAsync(impersonationToken);
            if (cacheItem == null)
            {
                throw new UserFriendlyException(L("ImpersonationTokenErrorMessage"));
            }

            CheckCurrentTenant(cacheItem.TargetTenantId);

            //Get the user from tenant
            var user = await _userManager.FindByIdAsync(cacheItem.TargetUserId);

            //Create identity
            var identity = await _userManager.CreateIdentityAsync(user, authenticationType);

            if (!cacheItem.IsBackToImpersonator)
            {
                //Add claims for audit logging
                if (cacheItem.ImpersonatorTenantId.HasValue)
                {
                    identity.AddClaim(new Claim(AbpClaimTypes.ImpersonatorTenantId, cacheItem.ImpersonatorTenantId.Value.ToString(CultureInfo.InvariantCulture)));
                }

                identity.AddClaim(new Claim(AbpClaimTypes.ImpersonatorUserId, cacheItem.ImpersonatorUserId.ToString(CultureInfo.InvariantCulture)));
            }

            //Remove the cache item to prevent re-use
            await _cacheManager.GetImpersonationCache().RemoveAsync(impersonationToken);

            return new UserAndIdentity(user, identity);
        }

        public Task<string> GetImpoersonateToken(long userId, int? tenantId)
        {
            if (AbpSession.ImpersonatorUserId.HasValue)
            {
                throw new UserFriendlyException(L("CascadeImpersonationErrorMessage"));
            }

            if (AbpSession.TenantId.HasValue)
            {
                if (!tenantId.HasValue)
                {
                    throw new UserFriendlyException(L("FromTenantToHostImpersonationErrorMessage"));
                }

                if (tenantId.Value != AbpSession.TenantId.Value)
                {
                    throw new UserFriendlyException(L("DifferentTenantImpersonationErrorMessage"));
                }
            }

            return GenerateImpersonationTokenAsync(tenantId, userId, false);
        }

        public Task<string> GetBackToImpersonatorToken()
        {
            if (!AbpSession.ImpersonatorUserId.HasValue)
            {
                throw new UserFriendlyException(L("NotImpersonatedLoginErrorMessage"));
            }

            return GenerateImpersonationTokenAsync(AbpSession.ImpersonatorTenantId, AbpSession.ImpersonatorUserId.Value, true);
        }

        private void CheckCurrentTenant(int? tenantId)
        {
            if (AbpSession.TenantId != tenantId)
            {
                throw new ApplicationException($"Current tenant is different than given tenant. AbpSession.TenantId: {AbpSession.TenantId}, given tenantId: {tenantId}");
            }
        }

        private async Task<string> GenerateImpersonationTokenAsync(int? tenantId, long userId, bool isBackToImpersonator)
        {
            //Create a cache item
            var cacheItem = new ImpersonationCacheItem(
                tenantId,
                userId,
                isBackToImpersonator
            );

            if (!isBackToImpersonator)
            {
                cacheItem.ImpersonatorTenantId = AbpSession.TenantId;
                cacheItem.ImpersonatorUserId = AbpSession.GetUserId();
            }

            //Create a random token and save to the cache
            var token = Guid.NewGuid().ToString();

            await _cacheManager
                .GetImpersonationCache()
                .SetAsync(token, cacheItem, TimeSpan.FromMinutes(1));

            return token;
        }
    }
}
