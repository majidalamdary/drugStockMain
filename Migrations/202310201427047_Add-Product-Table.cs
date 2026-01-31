namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductTable : DbMigration
    {
        public override void Up()
        {       
            
            
            
            CreateTable(
                "dbo.ProductTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            Sql("insert into ProductTypes (Title) values ('Tablet'),('Injection')");
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(nullable: false),
                        GenericCode = c.String(nullable: false),
                        Dose = c.String(),
                        MinimumCount = c.Long(nullable: false),
                        MaximumCount = c.Long(nullable: false),
                        BuyPrice = c.Long(),
                        SellPrice = c.Long(),
                        StoreId = c.Guid(nullable: false),
                        ProductTypeId = c.Int(nullable: false),
                        ProductSubGroupId = c.Guid(nullable: false),
                        CreatorUserId = c.String(maxLength: 128),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorUserId)
                .ForeignKey("dbo.ProductSubGroups", t => t.ProductSubGroupId, cascadeDelete: false)
                .ForeignKey("dbo.ProductTypes", t => t.ProductTypeId, cascadeDelete: false)
                .ForeignKey("dbo.Stores", t => t.StoreId, cascadeDelete: false)
                .Index(t => t.StoreId)
                .Index(t => t.ProductTypeId)
                .Index(t => t.ProductSubGroupId)
                .Index(t => t.CreatorUserId);
            

            
            CreateTable(
                "dbo.ProductTypeInProductSubGroups",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProductTypeId = c.Int(nullable: false),
                        ProductSubGroupId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductSubGroups", t => t.ProductSubGroupId, cascadeDelete: true)
                .ForeignKey("dbo.ProductTypes", t => t.ProductTypeId, cascadeDelete: true)
                .Index(t => t.ProductTypeId)
                .Index(t => t.ProductSubGroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductTypeInProductSubGroups", "ProductTypeId", "dbo.ProductTypes");
            DropForeignKey("dbo.ProductTypeInProductSubGroups", "ProductSubGroupId", "dbo.ProductSubGroups");
            DropForeignKey("dbo.Products", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.Products", "ProductTypeId", "dbo.ProductTypes");
            DropForeignKey("dbo.Products", "ProductSubGroupId", "dbo.ProductSubGroups");
            DropForeignKey("dbo.Products", "CreatorUserId", "dbo.AspNetUsers");
            DropIndex("dbo.ProductTypeInProductSubGroups", new[] { "ProductSubGroupId" });
            DropIndex("dbo.ProductTypeInProductSubGroups", new[] { "ProductTypeId" });
            DropIndex("dbo.Products", new[] { "CreatorUserId" });
            DropIndex("dbo.Products", new[] { "ProductSubGroupId" });
            DropIndex("dbo.Products", new[] { "ProductTypeId" });
            DropIndex("dbo.Products", new[] { "StoreId" });
            DropTable("dbo.ProductTypeInProductSubGroups");
            DropTable("dbo.ProductTypes");
            DropTable("dbo.Products");
        }
    }
}
