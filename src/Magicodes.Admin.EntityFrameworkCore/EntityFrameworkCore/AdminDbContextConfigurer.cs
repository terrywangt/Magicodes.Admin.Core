using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Magicodes.Admin.EntityFrameworkCore
{
    public static class AdminDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AdminDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
            //var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());
            ////以支持SQL Server 2012以下数据库
            //if (configuration.GetValue<bool>("UseRowNumberForPaging"))
            //    builder.UseSqlServer(connectionString, p => p.UseRowNumberForPaging());
            //else
            //    builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<AdminDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}