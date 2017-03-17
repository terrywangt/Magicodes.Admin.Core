namespace Magicodes.Admin.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Upgraded_Abp_v1_1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AbpUsers", "EmailConfirmationCode", c => c.String(maxLength: 328));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AbpUsers", "EmailConfirmationCode", c => c.String(maxLength: 128));
        }
    }
}
