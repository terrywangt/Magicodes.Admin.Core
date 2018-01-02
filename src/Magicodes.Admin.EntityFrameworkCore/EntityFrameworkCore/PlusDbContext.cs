using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Magicodes.Admin.EntityFrameworkCore
{
    public class PlusDbContext : AbpDbContext
    {
        static PlusDbContext()
        {
            
        }

        protected PlusDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
