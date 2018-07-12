using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Timing;
using Magicodes.Admin.Editions;
using Magicodes.Admin.MultiTenancy;
using Magicodes.Admin.MultiTenancy.Dto;
using Magicodes.Admin.MultiTenancy.Payments;
using Magicodes.Admin.MultiTenancy.Payments.Dto;
using Shouldly;
using Xunit;

namespace Magicodes.Admin.Tests.MultiTenancy
{
    public class SubscriptionManagement_Tests : AppTestBase
    {
        private readonly TenantManager _tenantManager;
        private readonly ITenantAppService _tenantAppService;
        private readonly EditionManager _editionManager;
        private readonly IPaymentAppService _paymentAppService;

     public SubscriptionManagement_Tests()
        {
            LoginAsHostAdmin();
            _tenantAppService = Resolve<ITenantAppService>();
            _tenantManager = Resolve<TenantManager>();
            _editionManager = Resolve<EditionManager>();
            _paymentAppService = Resolve<IPaymentAppService>();
        }

        [Theory]
        [InlineData(10, 10)]
        [InlineData(400, 400)]
        public void Calculate_Upgrade_To_Edition_Price(int remainingDaysCount, decimal upgradePrice)
        {
            var currentEdition = new SubscribableEdition()
            {
                DisplayName = "Standard",
                Name = "Standard",
                AnnualPrice = (int)PaymentPeriodType.Annual,
                MonthlyPrice = (int)PaymentPeriodType.Monthly
            };

            var targetEdition = new SubscribableEdition()
            {
                DisplayName = "Premium",
                Name = "Premium",
                AnnualPrice = (int)PaymentPeriodType.Annual * 2,
                MonthlyPrice = (int)PaymentPeriodType.Monthly * 2
            };

            decimal price;
            price = _tenantManager.GetUpgradePrice(currentEdition, targetEdition, remainingDaysCount);

            price.ShouldBe(upgradePrice);
        }

        [MultiTenantTheory]
        [InlineData(PaymentPeriodType.Annual, EditionPaymentType.Upgrade)]
        [InlineData(PaymentPeriodType.Monthly, EditionPaymentType.Upgrade)]
        public async Task Upgrade_Tenant_Edition(PaymentPeriodType paymentPeriodType,
            EditionPaymentType editionPaymentType)
        {
            var subscriptionEndDate = DateTime.Today.ToUniversalTime().AddDays(10);
            var updatedSubscriptionEndDate = subscriptionEndDate;

            await CreateUpdateTenant(paymentPeriodType, editionPaymentType, subscriptionEndDate, updatedSubscriptionEndDate);
        }

        [MultiTenantTheory]
        [InlineData(PaymentPeriodType.Annual, EditionPaymentType.BuyNow)]
        [InlineData(PaymentPeriodType.Monthly, EditionPaymentType.BuyNow)]
        public async Task BuyNow_Tenant_Edition(PaymentPeriodType paymentPeriodType,
            EditionPaymentType editionPaymentType)
        {
            DateTime? subscriptionEndDate = null;
            var updatedSubscriptionEndDate = Clock.Now.ToUniversalTime().AddDays((int)paymentPeriodType);

            await CreateUpdateTenant(paymentPeriodType, editionPaymentType, subscriptionEndDate, updatedSubscriptionEndDate);
        }

        [MultiTenantTheory]
        [InlineData(PaymentPeriodType.Annual, EditionPaymentType.Extend)]
        [InlineData(PaymentPeriodType.Monthly, EditionPaymentType.Extend)]
        public async Task Extend_Tenant_Edition(PaymentPeriodType paymentPeriodType,
            EditionPaymentType editionPaymentType)
        {
            var subscriptionEndDate = DateTime.Today.ToUniversalTime().AddDays(10);
            var updatedSubscriptionEndDate = subscriptionEndDate.AddDays((int)paymentPeriodType);

            await CreateUpdateTenant(paymentPeriodType, editionPaymentType, subscriptionEndDate, updatedSubscriptionEndDate);
        }

        [Fact]
        public async Task Mark_Tenant_As_Passive_When_Subscription_Expires()
        {
            //Act
            var utcNow = Clock.Now.ToUniversalTime();
            var freeEdition = new SubscribableEdition
            {
                DisplayName = "Free Edition"
            };
            var standard = new SubscribableEdition
            {
                DisplayName = "Standard Edition"
            };

            await UsingDbContextAsync(async context =>
            {
                context.SubscribableEditions.Add(freeEdition);
                context.SubscribableEditions.Add(standard);
                await context.SaveChangesAsync();
            });

            var tenant = new Tenant("AbpProjectName", "AbpProjectName")
            {
                EditionId = standard.Id,
                SubscriptionEndDateUtc = utcNow.AddDays(-1)
            };

            await UsingDbContextAsync(async context =>
            {
                context.Tenants.Add(tenant);
                await context.SaveChangesAsync();
            });

            var endSubscirptionResult = await _tenantManager.EndSubscriptionAsync(tenant, standard, utcNow);

            endSubscirptionResult.ShouldBe(EndSubscriptionResult.TenantSetInActive);

            UsingDbContext(context =>
            {
                var updatedTenant = context.Tenants.FirstOrDefault(t => t.Id == tenant.Id);
                updatedTenant.ShouldNotBe(null);
                updatedTenant.IsActive.ShouldBe(false);
            });
        }

        [Fact]
        public async Task Assing_Tenant_To_Another_Edition_If_ExpiringEdition_Is_Set()
        {
            //Act
            var utcNow = Clock.Now.ToUniversalTime();
            var freeEdition = new SubscribableEdition
            {
                DisplayName = "Free Edition"
            };
            var standard = new SubscribableEdition
            {
                DisplayName = "Standard Edition",
            };

            await UsingDbContextAsync(async context =>
            {
                context.SubscribableEditions.Add(freeEdition);
                await context.SaveChangesAsync();

                standard.ExpiringEditionId = freeEdition.Id;
                context.SubscribableEditions.Add(standard);
                await context.SaveChangesAsync();
            });

            var tenant = new Tenant("AbpProjectName", "AbpProjectName")
            {
                EditionId = standard.Id,
                SubscriptionEndDateUtc = utcNow.AddDays(-1)
            };

            await UsingDbContextAsync(async context =>
            {
                context.Tenants.Add(tenant);
                await context.SaveChangesAsync();
            });

            var endSubscirptionResult = await _tenantManager.EndSubscriptionAsync(tenant, standard, utcNow);

            endSubscirptionResult.ShouldBe(EndSubscriptionResult.AssignedToAnotherEdition);

            UsingDbContext(context =>
            {
                var updatedTenant = context.Tenants.FirstOrDefault(t => t.Id == tenant.Id);
                updatedTenant.ShouldNotBe(null);
                updatedTenant.IsActive.ShouldBe(true);
                updatedTenant.EditionId.ShouldBe(freeEdition.Id);
                updatedTenant.SubscriptionEndDateUtc.ShouldBe(null);
            });
        }

        [Fact]
        public async Task Keep_Tenant_Active_If_Subscription_Is_Not_Expired()
        {
            //Act
            var utcNow = Clock.Now.ToUniversalTime();
            var freeEdition = new SubscribableEdition
            {
                DisplayName = "Free Edition"
            };
            var standard = new SubscribableEdition
            {
                DisplayName = "Standard Edition",
            };

            await UsingDbContextAsync(async context =>
            {
                context.SubscribableEditions.Add(freeEdition);
                await context.SaveChangesAsync();

                context.SubscribableEditions.Add(standard);
                await context.SaveChangesAsync();
            });

            var tenant = new Tenant("AbpProjectName", "AbpProjectName")
            {
                EditionId = standard.Id,
                SubscriptionEndDateUtc = utcNow.AddDays(10)
            };

            await UsingDbContextAsync(async context =>
            {
                context.Tenants.Add(tenant);
                await context.SaveChangesAsync();
            });

            await Assert.ThrowsAsync<Exception>(async () => await _tenantManager.EndSubscriptionAsync(tenant, standard, utcNow));

            UsingDbContext(context =>
            {
                var updatedTenant = context.Tenants.FirstOrDefault(t => t.Id == tenant.Id);
                updatedTenant.ShouldNotBe(null);
                updatedTenant.IsActive.ShouldBe(true);
                updatedTenant.EditionId.ShouldBe(standard.Id);
                updatedTenant.SubscriptionEndDateUtc.ShouldNotBe(null);
            });
        }

        [Fact]
        public async Task Dont_Mark_Tenant_As_Passive_If_WaitingDayAfterExpire_Is_Not_Passed_And_Tenant_NotIsInTrialPeriod()
        {
            //Act
            var utcNow = Clock.Now.ToUniversalTime();
            var freeEdition = new SubscribableEdition
            {
                DisplayName = "Free Edition"
            };

            var standard = new SubscribableEdition
            {
                DisplayName = "Standard Edition",
                WaitingDayAfterExpire = 10
            };

            await UsingDbContextAsync(async context =>
            {
                context.SubscribableEditions.Add(freeEdition);
                await context.SaveChangesAsync();

                context.SubscribableEditions.Add(standard);
                await context.SaveChangesAsync();
            });

            var tenant = new Tenant("AbpProjectName", "AbpProjectName")
            {
                EditionId = standard.Id,
                SubscriptionEndDateUtc = utcNow.AddDays(-1)
            };

            await UsingDbContextAsync(async context =>
            {
                context.Tenants.Add(tenant);
                await context.SaveChangesAsync();
            });

            await Assert.ThrowsAsync<Exception>(async () => await _tenantManager.EndSubscriptionAsync(tenant, standard, utcNow));

            UsingDbContext(context =>
            {
                var updatedTenant = context.Tenants.FirstOrDefault(t => t.Id == tenant.Id);
                updatedTenant.ShouldNotBe(null);
                updatedTenant.IsActive.ShouldBe(true);
                updatedTenant.EditionId.ShouldBe(standard.Id);
                updatedTenant.SubscriptionEndDateUtc.ShouldNotBe(null);
            });
        }

        [Fact]
        public async Task Mark_Tenant_As_Passive_If_WaitingDayAfterExpire_Is_Not_Passed_And_Tenant_IsInTrialPeriod()
        {
            //Act
            var utcNow = Clock.Now.ToUniversalTime();
            var freeEdition = new SubscribableEdition
            {
                DisplayName = "Free Edition"
            };

            var standard = new SubscribableEdition
            {
                DisplayName = "Standard Edition",
                WaitingDayAfterExpire = 10
            };

            await UsingDbContextAsync(async context =>
            {
                context.SubscribableEditions.Add(freeEdition);
                await context.SaveChangesAsync();

                context.SubscribableEditions.Add(standard);
                await context.SaveChangesAsync();
            });

            var tenant = new Tenant("AbpProjectName", "AbpProjectName")
            {
                EditionId = standard.Id,
                SubscriptionEndDateUtc = utcNow.AddDays(-1),
                IsInTrialPeriod = true
            };

            await UsingDbContextAsync(async context =>
            {
                context.Tenants.Add(tenant);
                await context.SaveChangesAsync();
            });

            var endSubscriptionResult = await _tenantManager.EndSubscriptionAsync(tenant, standard, utcNow);

            UsingDbContext(context =>
            {
                var updatedTenant = context.Tenants.FirstOrDefault(t => t.Id == tenant.Id);
                updatedTenant.ShouldNotBe(null);
                updatedTenant.IsActive.ShouldBe(false);
                updatedTenant.EditionId.ShouldBe(standard.Id);
                endSubscriptionResult.ShouldBe(EndSubscriptionResult.TenantSetInActive);
                updatedTenant.SubscriptionEndDateUtc.ShouldNotBe(null);
            });
        }

        [Fact]
        public async Task Mark_Tenant_As_Passive_If_WaitingDayAfterExpire_Is_Passed()
        {
            //Act
            var utcNow = Clock.Now.ToUniversalTime();
            var freeEdition = new SubscribableEdition
            {
                DisplayName = "Free Edition"
            };

            var standard = new SubscribableEdition
            {
                DisplayName = "Standard Edition",
                WaitingDayAfterExpire = 10
            };

            await UsingDbContextAsync(async context =>
            {
                context.SubscribableEditions.Add(freeEdition);
                await context.SaveChangesAsync();

                context.SubscribableEditions.Add(standard);
                await context.SaveChangesAsync();
            });

            var tenant = new Tenant("AbpProjectName", "AbpProjectName")
            {
                EditionId = standard.Id,
                SubscriptionEndDateUtc = utcNow.AddDays(-11)
            };

            await UsingDbContextAsync(async context =>
            {
                context.Tenants.Add(tenant);
                await context.SaveChangesAsync();
            });

            var endSubscirptionResult = await _tenantManager.EndSubscriptionAsync(tenant, standard, utcNow);

            endSubscirptionResult.ShouldBe(EndSubscriptionResult.TenantSetInActive);

            UsingDbContext(context =>
            {
                var updatedTenant = context.Tenants.FirstOrDefault(t => t.Id == tenant.Id);
                updatedTenant.ShouldNotBe(null);
                updatedTenant.IsActive.ShouldBe(false);
            });
        }

        [Fact]
        public async Task GetPaymentHistory_Test()
        {
            LoginAsDefaultTenantAdmin();

            var result = await _paymentAppService.GetPaymentHistory(new GetPaymentHistoryInput());
            result.Items.Count.ShouldBe(2);
            result.Items[0].DayCount.ShouldBe(2);
            result.Items[1].DayCount.ShouldBe(29);
        }

        private async Task CreateUpdateTenant(PaymentPeriodType paymentPeriodType, EditionPaymentType editionPaymentType, DateTime? subscriptionEndDate, DateTime updatedSubscriptionEndDate)
        {
            await _editionManager.CreateAsync(new SubscribableEdition
            {
                Name = "CurrentEdition",
                DisplayName = "Current Edition"
            });

            await _editionManager.CreateAsync(new SubscribableEdition
            {
                Name = "TargetEdition",
                DisplayName = "Target Edition"
            });

            var currentEditionId = (await _editionManager.FindByNameAsync("CurrentEdition")).Id;
            var targetEditionId = (await _editionManager.FindByNameAsync("TargetEdition")).Id;

            // Create tenant
            await _tenantAppService.CreateTenant(
                new CreateTenantInput
                {
                    TenancyName = "testTenant",
                    Name = "Tenant for test purpose",
                    AdminEmailAddress = "admin@testtenant.com",
                    AdminPassword = "123qwe",
                    IsActive = true,
                    EditionId = currentEditionId,
                    SubscriptionEndDateUtc = subscriptionEndDate
                });

            //Assert
            var tenant = await GetTenantOrNullAsync("testTenant");
            tenant.ShouldNotBe(null);
            tenant.Name.ShouldBe("Tenant for test purpose");
            tenant.IsActive.ShouldBe(true);
            tenant.EditionId.ShouldBe(currentEditionId);
            tenant.SubscriptionEndDateUtc = subscriptionEndDate?.ToUniversalTime();

            // Update Tenant -----------------------------

            var tenantForUpdate = _tenantManager.UpdateTenantAsync(
                tenant.Id,
                true,
                false,
                paymentPeriodType,
                targetEditionId,
                editionPaymentType
            );

            //Assert
            tenant = await tenantForUpdate;
            tenant.IsActive.ShouldBe(true);
            tenant.IsInTrialPeriod.ShouldBe(false);
            tenant.EditionId.ShouldBe(targetEditionId);
            tenant.SubscriptionEndDateUtc.Value.Date.ShouldBe(updatedSubscriptionEndDate.ToUniversalTime().Date);
        }
    }
}
