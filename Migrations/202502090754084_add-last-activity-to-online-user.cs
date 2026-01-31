namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models.Constancts;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addlastactivitytoonlineuser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OnlineUsers", "LastActivityDateTime", c => c.DateTime(nullable: false));
            Sql("insert into UserStatus (Id,Title) values (" + (int)UserStatusValue.TempBlocked + ",N'محدود شده موقت')");
        }
        
        public override void Down()
        {
            DropColumn("dbo.OnlineUsers", "LastActivityDateTime");
        }
    }
}
