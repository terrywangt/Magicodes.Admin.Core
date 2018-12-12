using Abp.Timing;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Magicodes.Admin.Contents;
using Xunit;

namespace Magicodes.Admin.Tests.Contents
{
    public class ColumnInfoAppService_Tests : AppTestBase
    {
        private readonly IColumnInfoAppService _columnInfoAppService;

        public ColumnInfoAppService_Tests() => _columnInfoAppService = Resolve<IColumnInfoAppService>();

        [Fact]
        public async Task Should_DeleteAll_Logs()
        {
            //Arrange
            UsingDbContext(
                context =>
                {
                    context.ColumnInfos.Add(
                        new ColumnInfo
                        {
                            TenantId = AbpSession.TenantId,
                            Alias = "aa",
                            Code = "AA",
                            ColumnType = ColumnTypes.Html,
                            CreationTime = Clock.Now,
                            Description = "aa",
                            IsDeleted = false,
                            Title = "A",
                            KeyWords = "AA"
                        });

                    context.ColumnInfos.Add(
                        new ColumnInfo
                        {
                            TenantId = 5,
                            Alias = "nn",
                            Code = "bb",
                            ColumnType = ColumnTypes.Html,
                            CreationTime = Clock.Now,
                            Description = "bb",
                            IsDeleted = false,
                            Title = "b",
                            KeyWords = "bb"
                        });
                });
            var count = UsingDbContext(
                context => context.ColumnInfos.Count(p => !p.IsDeleted && p.TenantId == AbpSession.TenantId));
            count.ShouldBe(1);

            //Act
            await _columnInfoAppService.DeleteAll();

            count = UsingDbContext(
                context => context.ColumnInfos.Count(p => !p.IsDeleted && p.TenantId == AbpSession.TenantId));

            count.ShouldBe(0);
        }


    }
}
