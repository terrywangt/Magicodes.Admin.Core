using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Net.Mail;
using Abp.Threading;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Editions;

namespace Magicodes.Admin.MultiTenancy
{
    public class SubscriptionExpirationCheckWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int CheckPeriodAsMilliseconds = 1 * 60 * 60 * 1000; //1 hour

        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<User, long> _useRepository;
        private readonly IEmailSender _emailSender;
        private readonly ISettingManager _settingManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<SubscribableEdition> _editionRepository;
        private readonly TenantManager _tenantManager;

        public SubscriptionExpirationCheckWorker(
            AbpTimer timer,
            IRepository<Tenant> tenantRepository,
            IEmailSender emailSender,
            IRepository<User, long> useRepository,
            ISettingManager settingManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<SubscribableEdition> editionRepository,
            TenantManager tenantManager)
            : base(timer)
        {
            _tenantRepository = tenantRepository;
            _emailSender = emailSender;
            _useRepository = useRepository;
            _settingManager = settingManager;
            _unitOfWorkManager = unitOfWorkManager;
            _editionRepository = editionRepository;
            _tenantManager = tenantManager;

            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = true;

            LocalizationSourceName = AdminConsts.LocalizationSourceName;
        }

        protected override void DoWork()
        {
            var utcNow = Clock.Now.ToUniversalTime();
            var failedTenancyNames = new List<string>();

            var subscriptionExpiredTenants = _tenantRepository.GetAllList(
                tenant => tenant.SubscriptionEndDateUtc != null &&
                          tenant.SubscriptionEndDateUtc <= utcNow &&
                          tenant.IsActive &&
                          tenant.EditionId != null
            );

            foreach (var tenant in subscriptionExpiredTenants)
            {
                Debug.Assert(tenant.EditionId.HasValue);

                try
                {

                    var edition = _editionRepository.Get(tenant.EditionId.Value);

                    Debug.Assert(tenant.SubscriptionEndDateUtc != null, "tenant.SubscriptionEndDateUtc != null");

                    if (tenant.SubscriptionEndDateUtc.Value.AddDays(edition.WaitingDayAfterExpire ?? 0) >= utcNow)
                    {
                        //Tenant is in waiting days after expire TODO: It's better to filter such entities while querying from repository!
                        continue;
                    }

                    var endSubscriptionResult = AsyncHelper.RunSync(() => _tenantManager.EndSubscriptionAsync(tenant, edition, utcNow));

                    if (endSubscriptionResult == EndSubscriptionResult.TenantSetInActive)
                    {
                        TryToSendSubscriptionExpireEmail(tenant.Id, utcNow);
                    }
                    else if (endSubscriptionResult == EndSubscriptionResult.AssignedToAnotherEdition)
                    {
                        //TODO: We can send an email in this case too
                    }
                }
                catch (Exception exception)
                {
                    failedTenancyNames.Add(tenant.TenancyName);
                    Logger.Error($"Subscription of tenant {tenant.TenancyName} has been expired but tenant couldn't be made passive !");
                    Logger.Error(exception.Message, exception);
                }
            }

            if (!failedTenancyNames.Any())
            {
                return;
            }

            TryToSendFailedSubscriptionTerminationsEmail(failedTenancyNames, utcNow);
        }

        private void TryToSendSubscriptionExpireEmail(int tenantId, DateTime utcNow)
        {
            try
            {
                using (_unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        var tenantAdmin = _useRepository.FirstOrDefault(u => u.UserName == AbpUserBase.AdminUserName);
                        if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
                        {
                            return;
                        }

                        var hostAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, tenantAdmin.TenantId, tenantAdmin.Id);
                        var culture = GetCultureInfoByChecking(hostAdminLanguage);

                        var subject = L("SubscriptionExpire_Email_Subject", culture);
                        var body = L("SubscriptionExpire_Email_Body", culture, utcNow.ToString("yyyy-MM-dd") + " UTC");
                        _emailSender.Send(tenantAdmin.EmailAddress, subject, body);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        private void TryToSendFailedSubscriptionTerminationsEmail(List<string> failedTenancyNames, DateTime utcNow)
        {
            try
            {
                var hostAdmin = _useRepository.FirstOrDefault(u => u.UserName == AbpUserBase.AdminUserName);
                if (hostAdmin == null || string.IsNullOrEmpty(hostAdmin.EmailAddress))
                {
                    return;
                }

                var hostAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, hostAdmin.TenantId, hostAdmin.Id);
                var culture = GetCultureInfoByChecking(hostAdminLanguage);

                var subject = L("FailedSubscriptionTerminations_Email_Subject", culture);
                var body = L("FailedSubscriptionTerminations_Email_Body", culture, string.Join(",", failedTenancyNames), utcNow.ToString("yyyy-MM-dd") + " UTC");
                _emailSender.Send(hostAdmin.EmailAddress, subject, body);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        private static CultureInfo GetCultureInfoByChecking(string name)
        {
            try
            {
                return CultureInfoHelper.Get(name);
            }
            catch (CultureNotFoundException)
            {
                return CultureInfo.CurrentCulture;
            }
        }
    }
}
