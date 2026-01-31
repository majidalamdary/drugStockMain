namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class security_setting_logout_inactive_session : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SecuritySettings", "LogOutInActiveSession", c => c.Int(nullable: false,defaultValue:15));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SecuritySettings", "LogOutInActiveSession");
        }
    }
}
