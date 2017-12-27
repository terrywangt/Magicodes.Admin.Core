using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Magicodes.Admin.EntityFrameworkCore
{
    public static class AdminDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AdminDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<AdminDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}