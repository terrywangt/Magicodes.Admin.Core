using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.Web;

namespace Magicodes.Admin.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class AdminDbContextFactory : IDbContextFactory<AdminDbContext>
    {
        public AdminDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<AdminDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            AdminDbContextConfigurer.Configure(builder, configuration.GetConnectionString(AdminConsts.ConnectionStringName));
            
            return new AdminDbContext(builder.Options);
        }
    }
}