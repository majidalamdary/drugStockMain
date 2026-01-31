using DrugStockWeb.Models.Constancts;

namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newaftamigration3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FailedLoginDateTime", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "SuccessLoginDateTime", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "BlockedDateTime", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "Email", c => c.String());
            AddColumn("dbo.AspNetUsers", "UserStatusId", c => c.Int(defaultValue:(int)UserStatusValue.Activated));
            CreateIndex("dbo.AspNetUsers", "UserStatusId");
            AddForeignKey("dbo.AspNetUsers", "UserStatusId", "dbo.UserStatus", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "UserStatusId", "dbo.UserStatus");
            DropIndex("dbo.AspNetUsers", new[] { "UserStatusId" });
            DropColumn("dbo.AspNetUsers", "UserStatusId");
            DropColumn("dbo.AspNetUsers", "Email");
            DropColumn("dbo.AspNetUsers", "BlockedDateTime");
            DropColumn("dbo.AspNetUsers", "SuccessLoginDateTime");
            DropColumn("dbo.AspNetUsers", "FailedLoginDateTime");
        }
    }
}
