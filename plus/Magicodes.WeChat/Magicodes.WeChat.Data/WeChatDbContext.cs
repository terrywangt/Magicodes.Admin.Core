using Abp.EntityFramework;
using Magicodes.Admin.EntityFramework;
using Magicodes.WeChat.Core.User;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeChat.Data
{
    [DbConfigurationType(typeof(WeChatDbConfiguration))]
    public class WeChatDbContext : AbpDbContext
    {
        public virtual IDbSet<User_WeChatUser> User_WeChatUsers { get; set; }

        public WeChatDbContext() : base()
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
