namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addStoreReceipReturn : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StoreReceiptReturns",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ReceiptReturnDate = c.DateTime(nullable: false),
                        IsConfirmed = c.Boolean(nullable: false),
                        Describ = c.String(),
                        CreatorUserId = c.String(maxLength: 128),
                        ConfirmerUserId = c.String(maxLength: 128),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                        StoreReceiptId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ConfirmerUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorUserId)
                .ForeignKey("dbo.StoreReceipts", t => t.StoreReceiptId, cascadeDelete: true)
                .Index(t => t.CreatorUserId)
                .Index(t => t.ConfirmerUserId)
                .Index(t => t.StoreReceiptId);
            
            CreateTable(
                "dbo.StoreReceiptDetailReturns",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StoreReceiptDetailId = c.Guid(nullable: false),
                        StoreReceiptReturnId = c.Guid(nullable: false),
                        Count = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StoreReceiptDetails", t => t.StoreReceiptDetailId, cascadeDelete: true)
                .ForeignKey("dbo.StoreReceiptReturns", t => t.StoreReceiptReturnId, cascadeDelete: true)
                .Index(t => t.StoreReceiptDetailId)
                .Index(t => t.StoreReceiptReturnId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoreReceiptDetailReturns", "StoreReceiptReturnId", "dbo.StoreReceiptReturns");
            DropForeignKey("dbo.StoreReceiptDetailReturns", "StoreReceiptDetailId", "dbo.StoreReceiptDetails");
            DropForeignKey("dbo.StoreReceiptReturns", "StoreReceiptId", "dbo.StoreReceipts");
            DropForeignKey("dbo.StoreReceiptReturns", "CreatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.StoreReceiptReturns", "ConfirmerUserId", "dbo.AspNetUsers");
            DropIndex("dbo.StoreReceiptDetailReturns", new[] { "StoreReceiptReturnId" });
            DropIndex("dbo.StoreReceiptDetailReturns", new[] { "StoreReceiptDetailId" });
            DropIndex("dbo.StoreReceiptReturns", new[] { "StoreReceiptId" });
            DropIndex("dbo.StoreReceiptReturns", new[] { "ConfirmerUserId" });
            DropIndex("dbo.StoreReceiptReturns", new[] { "CreatorUserId" });
            DropTable("dbo.StoreReceiptDetailReturns");
            DropTable("dbo.StoreReceiptReturns");
        }
    }
}
