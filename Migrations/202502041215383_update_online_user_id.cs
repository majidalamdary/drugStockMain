namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_online_user_id : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.OnlineUsers", new[] { "User_Id" });
            DropColumn("dbo.OnlineUsers", "UserId");
            RenameColumn(table: "dbo.OnlineUsers", name: "User_Id", newName: "UserId");
            AddColumn("dbo.OnlineUsers", "SessionId", c => c.String());
            AlterColumn("dbo.OnlineUsers", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.OnlineUsers", "UserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.OnlineUsers", new[] { "UserId" });
            AlterColumn("dbo.OnlineUsers", "UserId", c => c.Guid(nullable: false));
            DropColumn("dbo.OnlineUsers", "SessionId");
            RenameColumn(table: "dbo.OnlineUsers", name: "UserId", newName: "User_Id");
            AddColumn("dbo.OnlineUsers", "UserId", c => c.Guid(nullable: false));
            CreateIndex("dbo.OnlineUsers", "User_Id");
        }
    }
}
