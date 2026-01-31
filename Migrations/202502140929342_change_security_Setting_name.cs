namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change_security_Setting_name : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.LogSettings", newName: "SecuritySettings");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.SecuritySettings", newName: "LogSettings");
        }
    }
}
