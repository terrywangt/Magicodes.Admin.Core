using Magicodes.Admin.EntityFramework;

namespace Magicodes.Admin.Migrations.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly AdminDbContext _context;

        public InitialHostDbBuilder(AdminDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}
