namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addStoreReceiptDetailsDetailTempstable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StoreReceiptDetailTemps",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                        BuyPrice = c.Long(nullable: false),
                        SellPrice = c.Long(nullable: false),
                        ExpireDate = c.DateTime(nullable: false),
                        BatchNumber = c.String(),
                        StoreReceiptId = c.Guid(nullable: false),
                        Count = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: false)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoreReceiptDetailTemps", "ProductId", "dbo.Products");
            DropIndex("dbo.StoreReceiptDetailTemps", new[] { "ProductId" });
            DropTable("dbo.StoreReceiptDetailTemps");
        }
    }
}
