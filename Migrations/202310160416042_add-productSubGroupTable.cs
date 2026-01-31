namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addproductSubGroupTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductSubGroups",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(nullable: false),
                        ProductGroupId = c.Guid(nullable: false),
                        CreatorUserId = c.String(maxLength: 128),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorUserId)
                .ForeignKey("dbo.ProductGroups", t => t.ProductGroupId, cascadeDelete: false)
                .Index(t => t.ProductGroupId)
                .Index(t => t.CreatorUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductSubGroups", "ProductGroupId", "dbo.ProductGroups");
            DropForeignKey("dbo.ProductSubGroups", "CreatorUserId", "dbo.AspNetUsers");
            DropIndex("dbo.ProductSubGroups", new[] { "CreatorUserId" });
            DropIndex("dbo.ProductSubGroups", new[] { "ProductGroupId" });
            DropTable("dbo.ProductSubGroups");
        }
    }
}
