using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Microsoft.EntityFrameworkCore;
using Magicodes.Admin.Editions;
using Magicodes.Admin.Editions.Dto;
using Magicodes.Admin.Features;
using Shouldly;

namespace Magicodes.Admin.Tests.Editions
{
    public class EditionAppService_Tests : AppTestBase
    {
        private readonly IEditionAppService _editionAppService;

        public EditionAppService_Tests()
        {
            LoginAsHostAdmin();
            _editionAppService = Resolve<IEditionAppService>();
        }

        [MultiTenantFact]
        public async Task Should_Get_Editions()
        {
            var editions = await _editionAppService.GetEditions();
            editions.Items.Count.ShouldBeGreaterThan(0);
        }

        [MultiTenantFact]
        public async Task Should_Create_Edition()
        {
            //Getting edition for edit
            var output = await _editionAppService.GetEditionForEdit(new NullableIdDto(null));

            //Changing a sample feature value
            var chatFeature = output.FeatureValues.FirstOrDefault(f => f.Name == AppFeatures.ChatFeature);
            if (chatFeature != null)
            {
                chatFeature.Value = chatFeature.Value = "true";
            }

            await _editionAppService.CreateOrUpdateEdition(
                new CreateOrUpdateEditionDto
                {
                    Edition = new EditionEditDto
                    {
                        DisplayName = "Premium Edition"
                    },
                    FeatureValues = output.FeatureValues
                });

            await UsingDbContextAsync(async context =>
            {
                var premiumEditon = await context.Editions.FirstOrDefaultAsync(e => e.DisplayName == "Premium Edition");
                premiumEditon.ShouldNotBeNull();

                if (chatFeature != null)
                {
                    var sampleFeatureValue = context.EditionFeatureSettings.FirstOrDefault(s => s.EditionId == premiumEditon.Id && s.Name == AppFeatures.ChatFeature);
                    sampleFeatureValue.ShouldNotBe(null);
                    sampleFeatureValue.Value.ShouldBe("true");
                }
            });
        }

        [MultiTenantFact]
        public async Task Should_Create_Subscribable_Edition()
        {
            var editionName = "Premium Edition";
            var monthlyPrice = 10;
            var annualPrice = 100;

            await _editionAppService.CreateOrUpdateEdition(
                new CreateOrUpdateEditionDto
                {
                    Edition = new EditionEditDto
                    {
                        DisplayName = editionName,
                        MonthlyPrice = monthlyPrice,
                        AnnualPrice = annualPrice,
                    },
                    FeatureValues = new List<NameValueDto>()
                });

            var editionRecord = UsingDbContext(context => context.Editions.FirstOrDefault(e => e.DisplayName == editionName));
            var output = await _editionAppService.GetEditionForEdit(new NullableIdDto(editionRecord.Id));

            var premiumEditon = output.Edition;
            premiumEditon.ShouldNotBeNull();

            premiumEditon.MonthlyPrice.ShouldBe(monthlyPrice);
            premiumEditon.AnnualPrice.ShouldBe(annualPrice);
        }

        [MultiTenantFact]
        public async Task Should_Update_Edition()
        {
            var defaultEdition = UsingDbContext(context => context.Editions.FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName));
            defaultEdition.ShouldNotBeNull();

            var output = await _editionAppService.GetEditionForEdit(new NullableIdDto(defaultEdition.Id));

            //Changing a sample feature value
            var chatFeature = output.FeatureValues.FirstOrDefault(f => f.Name == AppFeatures.ChatFeature);
            if (chatFeature != null)
            {
                chatFeature.Value = chatFeature.Value = "true";
            }

            await _editionAppService.CreateOrUpdateEdition(
                new CreateOrUpdateEditionDto
                {
                    Edition = new EditionEditDto
                    {
                        Id = output.Edition.Id,
                        DisplayName = "Regular Edition"
                    },
                    FeatureValues = output.FeatureValues
                });

            UsingDbContext(context =>
            {
                defaultEdition = context.Editions.FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
                defaultEdition.DisplayName.ShouldBe("Regular Edition");
            });
        }

        [MultiTenantFact]
        public async Task Should_Delete_Edition()
        {
            var editions = await _editionAppService.GetEditions();
            editions.Items.Count.ShouldBeGreaterThan(0);

            var defaultEdition = UsingDbContext(context => context.Editions.FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName));
            await _editionAppService.DeleteEdition(new EntityDto(defaultEdition.Id));

            UsingDbContext(context =>
            {
                defaultEdition = context.Editions.FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
                defaultEdition.ShouldNotBeNull();
                defaultEdition.IsDeleted.ShouldBe(true);

                context.Tenants.Count(t => t.EditionId == defaultEdition.Id).ShouldBe(0);
            });
        }
    }
}
