using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Magicodes.Admin.EntityFrameworkCore
{
    public static class AdminDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AdminDbContext> builder, string connectionString)
        {
            builder.UseMySql(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<AdminDbContext> builder, DbConnection connection)
        {
            builder.UseMySql(connection);
        }
        //public static void Configure(DbContextOptionsBuilder<AdminDbContext> builder, string connectionString, bool isUseRowNumber = true)
        //{
        //    if (isUseRowNumber)
        //    {
        //        //��֧��SQL Server 2012�������ݿ�
        //        builder.UseSqlServer(connectionString, p => p.UseRowNumberForPaging());
        //    }
        //    else
        //    {
        //        builder.UseSqlServer(connectionString);
        //    }
        //}

        //public static void Configure(DbContextOptionsBuilder<AdminDbContext> builder, DbConnection connection, bool isUseRowNumber = true)
        //{
        //    if (isUseRowNumber)
        //    {
        //        //��֧��SQL Server 2012�������ݿ�
        //        builder.UseSqlServer(connection, p => p.UseRowNumberForPaging());
        //    }
        //    else
        //    {
        //        builder.UseSqlServer(connection);
        //    }
        //}
    }
}