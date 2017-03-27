using Abp.EntityFramework;
using Magicodes.Admin;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.EntityFramework;
using Magicodes.Admin.Web;
using Magicodes.WeChat.Core.User;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeChat.Data
{
    //[DbConfigurationType(typeof(WeChatDbConfiguration))]
    [DbConfigurationType(typeof(AdminDbConfiguration))]
    public class WeChatDbContext : AbpDbContext
    {
        public virtual IDbSet<WeChatUser> User_WeChatUsers { get; set; }

        public WeChatDbContext() : base(GetConnectionString())
        {
        }

        public WeChatDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        public WeChatDbContext(DbConnection existingConnection)
            : base(existingConnection, false)
        {

        }

        public WeChatDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Types().Configure(entity => entity.ToTable("MWC." + entity.ClrType.Name));
        }
        private static string GetConnectionString()
        {
            //从配置中获取连接字符串
            var configuration = AppConfigurations.Get(
                WebContentDirectoryFinder.CalculateContentRootFolder()
                );

            return configuration.GetConnectionString(
                AdminConsts.ConnectionStringName
                );
        }

    }
    public class WeChatDbConfiguration : DbConfiguration
    {
        public WeChatDbConfiguration()
        {
            SetProviderServices(
                "System.Data.SqlClient",
                System.Data.Entity.SqlServer.SqlProviderServices.Instance
            );
        }
    }
}
