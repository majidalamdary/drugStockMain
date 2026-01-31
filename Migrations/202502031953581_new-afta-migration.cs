namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newaftamigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OnlineUsers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        LoginDateTime = c.DateTime(nullable: false),
                        Browser = c.String(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.LogSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FailedLoginMaxTryingTime = c.Int(nullable: false),
                        ActiveUserAfterTimePeriodByMinutes = c.Int(nullable: false),
                        MinPasswordLength = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: false),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OnlineUsers", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.OnlineUsers", new[] { "User_Id" });
            DropTable("dbo.UserStatus");
            DropTable("dbo.LogSettings");
            DropTable("dbo.OnlineUsers");
        }
    }
}
