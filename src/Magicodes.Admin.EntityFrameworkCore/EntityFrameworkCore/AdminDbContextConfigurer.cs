using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Magicodes.Admin.EntityFrameworkCore
{
    public static class AdminDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AdminDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }
    }
}