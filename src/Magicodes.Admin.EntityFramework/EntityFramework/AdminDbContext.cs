using System.Data.Common;
using System.Data.Entity;
using Abp.Zero.EntityFramework;
using Microsoft.Extensions.Configuration;
using Magicodes.Admin.Authorization.Roles;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Chat;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.Friendships;
using Magicodes.Admin.MultiTenancy;
using Magicodes.Admin.Storage;
using Magicodes.Admin.Web;

namespace Magicodes.Admin.EntityFramework
{
    /* Constructors of this DbContext is important and each one has it's own use case.
     * - Default constructor is used by EF tooling on development time.
     * - constructor(nameOrConnectionString) is used by ABP on runtime.
     * - constructor(existingConnection) is used by unit tests.
     * - constructor(existingConnection,contextOwnsConnection) can be used by ABP if DbContextEfTransactionStrategy is used.
     * See http://www.aspnetboilerplate.com/Pages/Documents/EntityFramework-Integration for more.
     */

    [DbConfigurationType(typeof(AdminDbConfiguration))]
    public class AdminDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        /* Define an IDbSet for each entity of the application */

        public virtual IDbSet<BinaryObject> BinaryObjects { get; set; }

        public virtual IDbSet<Friendship> Friendships { get; set; }

        public virtual IDbSet<ChatMessage> ChatMessages { get; set; }

        public AdminDbContext()
            : base(GetConnectionString())
        {

        }

        private static string GetConnectionString()
        {
            //Notice that; this logic only works on development time.
            //It is used to get connection string from appsettings.json in the Web project.

            var configuration = AppConfigurations.Get(
                WebContentDirectoryFinder.CalculateContentRootFolder()
                );

            return configuration.GetConnectionString(
                AdminConsts.ConnectionStringName
                );
        }

        public AdminDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        public AdminDbContext(DbConnection existingConnection)
            : base(existingConnection, false)
        {

        }

        public AdminDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {

        }
    }

    public class AdminDbConfiguration : DbConfiguration
    {
        public AdminDbConfiguration()
        {
            SetProviderServices(
                "System.Data.SqlClient",
                System.Data.Entity.SqlServer.SqlProviderServices.Instance
            );
        }
    }
}
