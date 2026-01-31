namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateStoreReceiptTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StoreReceipts", "CreatorUserId", "dbo.AspNetUsers");
            CreateTable(
                "dbo.StoreReceiptDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                        BuyPrice = c.Long(nullable: false),
                        SellPrice = c.Long(nullable: false),
                        ExpireDate = c.DateTime(nullable: false),
                        BatchNumber = c.String(),
                        Count = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: false)
                .Index(t => t.ProductId);
            
            AddColumn("dbo.StoreReceipts", "ConfirmerUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.StoreReceipts", "User_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.StoreReceipts", "Title", c => c.String());
            CreateIndex("dbo.StoreReceipts", "ConfirmerUserId");
            CreateIndex("dbo.StoreReceipts", "User_Id");
            AddForeignKey("dbo.StoreReceipts", "ConfirmerUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.StoreReceipts", "User_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoreReceipts", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.StoreReceiptDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.StoreReceipts", "ConfirmerUserId", "dbo.AspNetUsers");
            DropIndex("dbo.StoreReceiptDetails", new[] { "ProductId" });
            DropIndex("dbo.StoreReceipts", new[] { "User_Id" });
            DropIndex("dbo.StoreReceipts", new[] { "ConfirmerUserId" });
            AlterColumn("dbo.StoreReceipts", "Title", c => c.String(nullable: false));
            DropColumn("dbo.StoreReceipts", "User_Id");
            DropColumn("dbo.StoreReceipts", "ConfirmerUserId");
            DropTable("dbo.StoreReceiptDetails");
            AddForeignKey("dbo.StoreReceipts", "CreatorUserId", "dbo.AspNetUsers", "Id");
        }
    }
}
