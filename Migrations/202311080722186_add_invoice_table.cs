namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_invoice_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FactorNumber = c.String(nullable: false),
                        FactorDate = c.DateTime(nullable: false),
                        IsConfirmed = c.Boolean(nullable: false),
                        Describe = c.String(),
                        BusinnessPartnerId = c.Guid(nullable: false),
                        StoreReceiptId = c.Guid(nullable: false),
                        CreatorUserId = c.String(maxLength: 128),
                        ConfirmerUserId = c.String(maxLength: 128),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                        Invoice_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BusinnessPartners", t => t.BusinnessPartnerId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.ConfirmerUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorUserId)
                .ForeignKey("dbo.Invoices", t => t.Invoice_Id)
                .ForeignKey("dbo.StoreReceipts", t => t.StoreReceiptId, cascadeDelete: false)
                .Index(t => t.BusinnessPartnerId)
                .Index(t => t.StoreReceiptId)
                .Index(t => t.CreatorUserId)
                .Index(t => t.ConfirmerUserId)
                .Index(t => t.Invoice_Id);
            
            CreateTable(
                "dbo.InvoiceDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                        SellPrice = c.Long(nullable: false),
                        InvoiceId = c.Guid(nullable: false),
                        Count = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Invoices", t => t.InvoiceId, cascadeDelete: false)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: false)
                .Index(t => t.ProductId)
                .Index(t => t.InvoiceId);
            
            CreateTable(
                "dbo.InvoiceDetailTemps",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                        SellPrice = c.Long(nullable: false),
                        Count = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: false)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InvoiceDetailTemps", "ProductId", "dbo.Products");
            DropForeignKey("dbo.InvoiceDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.InvoiceDetails", "InvoiceId", "dbo.Invoices");
            DropForeignKey("dbo.Invoices", "StoreReceiptId", "dbo.StoreReceipts");
            DropForeignKey("dbo.Invoices", "Invoice_Id", "dbo.Invoices");
            DropForeignKey("dbo.Invoices", "CreatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Invoices", "ConfirmerUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Invoices", "BusinnessPartnerId", "dbo.BusinnessPartners");
            DropIndex("dbo.InvoiceDetailTemps", new[] { "ProductId" });
            DropIndex("dbo.InvoiceDetails", new[] { "InvoiceId" });
            DropIndex("dbo.InvoiceDetails", new[] { "ProductId" });
            DropIndex("dbo.Invoices", new[] { "Invoice_Id" });
            DropIndex("dbo.Invoices", new[] { "ConfirmerUserId" });
            DropIndex("dbo.Invoices", new[] { "CreatorUserId" });
            DropIndex("dbo.Invoices", new[] { "StoreReceiptId" });
            DropIndex("dbo.Invoices", new[] { "BusinnessPartnerId" });
            DropTable("dbo.InvoiceDetailTemps");
            DropTable("dbo.InvoiceDetails");
            DropTable("dbo.Invoices");
        }
    }
}
