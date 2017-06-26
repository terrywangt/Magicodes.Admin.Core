using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using Magicodes.Admin.Chat.SignalR;
using Magicodes.Admin.Editions;
using Magicodes.Admin.Sessions.Dto;

namespace Magicodes.Admin.Sessions
{
    public class SessionAppService : AdminAppServiceBase, ISessionAppService
    {
        [DisableAuditing]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = AppVersionHelper.Version,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>
                    {
                        { "SignalR", SignalRFeature.IsAvailable }
                    }
                }
            };

            if (AbpSession.TenantId.HasValue)
            {
                var tenant = await TenantManager.Tenants.Include(t => t.Edition)
                    .FirstAsync(t => t.Id == AbpSession.GetTenantId());

                var highestEdition = ObjectMapper.Map<List<SubscribableEdition>>(TenantManager.EditionManager.Editions)
                    .OrderByDescending(e => e.MonthlyPrice).FirstOrDefault();

                output.Tenant = ObjectMapper.Map<TenantLoginInfoDto>(tenant);
                output.Tenant.Edition.IsHighestEdition = highestEdition != null && output.Tenant.Edition.Id == highestEdition.Id;
            }

            if (AbpSession.UserId.HasValue)
            {
                output.User = ObjectMapper.Map<UserLoginInfoDto>(await GetCurrentUserAsync());
            }

            if (output.Tenant == null)
            {
                return output;
            }

            output.Tenant.SubscriptionDateString = GetTenantSubscriptionDateString(output);
            output.Tenant.CreationTimeString = output.Tenant.CreationTime.ToString("d");

            return output;
        }

        private string GetTenantSubscriptionDateString(GetCurrentLoginInformationsOutput output)
        {
            return output.Tenant.SubscriptionEndDateUtc == null
                ? L("Unlimited")
                : output.Tenant.SubscriptionEndDateUtc?.ToString("d");
        }

        public async Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken()
        {
            if (AbpSession.UserId <= 0)
            {
                throw new Exception(L("ThereIsNoLoggedInUser"));
            }

            var user = await UserManager.GetUserAsync(AbpSession.ToUserIdentifier());
            user.SetSignInToken();
            return new UpdateUserSignInTokenOutput
            {
                SignInToken = user.SignInToken,
                EncodedUserId = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Id.ToString())),
                EncodedTenantId = user.TenantId.HasValue ? Convert.ToBase64String(Encoding.UTF8.GetBytes(user.TenantId.Value.ToString())) : ""
            };
        }
    }
}