using Magicodes.Admin.EntityFrameworkCore;

namespace Magicodes.Admin.Tests.TestDatas
{
    public class TestDataBuilder
    {
        private readonly AdminDbContext _context;
        private readonly int _tenantId;

        public TestDataBuilder(AdminDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            new TestOrganizationUnitsBuilder(_context, _tenantId).Create();

            _context.SaveChanges();
        }
    }
}
