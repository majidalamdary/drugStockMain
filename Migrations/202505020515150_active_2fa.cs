namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class active_2fa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SecuritySettings", "Active2Fa", c => c.Boolean(nullable: false));
            AddColumn("dbo.SecuritySettings", "DbTampered", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SecuritySettings", "DbTampered");
            DropColumn("dbo.SecuritySettings", "Active2Fa");
        }
    }
}
