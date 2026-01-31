namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addstore_in_usertable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo." +
                "StoreInUsers" +
                "",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.String(maxLength: 128),
                        StoreId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stores", t => t.StoreId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.StoreId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoreInUsers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.StoreInUsers", "StoreId", "dbo.Stores");
            DropIndex("dbo.StoreInUsers", new[] { "StoreId" });
            DropIndex("dbo.StoreInUsers", new[] { "UserId" });
            DropTable("dbo.StoreInUsers");
        }
    }
}
