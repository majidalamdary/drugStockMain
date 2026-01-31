namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_invoice_table3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Invoices", "StoreReceiptId", "dbo.StoreReceipts");
            DropForeignKey("dbo.InvoiceDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.InvoiceDetailTemps", "ProductId", "dbo.Products");
            DropIndex("dbo.Invoices", new[] { "StoreReceiptId" });
            DropIndex("dbo.InvoiceDetails", new[] { "ProductId" });
            DropIndex("dbo.InvoiceDetailTemps", new[] { "ProductId" });
            AddColumn("dbo.InvoiceDetails", "StoreReceiptDetailId", c => c.Guid(nullable: false));
            AddColumn("dbo.InvoiceDetails", "StoreReceipt_Id", c => c.Guid());
            AddColumn("dbo.InvoiceDetailTemps", "StoreReceiptDetailId", c => c.Guid(nullable: false));
            AddColumn("dbo.InvoiceDetailTemps", "StoreReceipt_Id", c => c.Guid());
            CreateIndex("dbo.InvoiceDetails", "StoreReceiptDetailId");
            CreateIndex("dbo.InvoiceDetails", "StoreReceipt_Id");
            CreateIndex("dbo.InvoiceDetailTemps", "StoreReceiptDetailId");
            CreateIndex("dbo.InvoiceDetailTemps", "StoreReceipt_Id");
            AddForeignKey("dbo.InvoiceDetails", "StoreReceiptDetailId", "dbo.StoreReceiptDetails", "Id", cascadeDelete: false);
            AddForeignKey("dbo.InvoiceDetails", "StoreReceipt_Id", "dbo.StoreReceipts", "Id");
            AddForeignKey("dbo.InvoiceDetailTemps", "StoreReceiptDetailId", "dbo.StoreReceiptDetails", "Id", cascadeDelete: false);
            AddForeignKey("dbo.InvoiceDetailTemps", "StoreReceipt_Id", "dbo.StoreReceipts", "Id");
            DropColumn("dbo.Invoices", "StoreReceiptId");
            DropColumn("dbo.InvoiceDetails", "ProductId");
            DropColumn("dbo.InvoiceDetailTemps", "ProductId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InvoiceDetailTemps", "ProductId", c => c.Guid(nullable: false));
            AddColumn("dbo.InvoiceDetails", "ProductId", c => c.Guid(nullable: false));
            AddColumn("dbo.Invoices", "StoreReceiptId", c => c.Guid(nullable: false));
            DropForeignKey("dbo.InvoiceDetailTemps", "StoreReceipt_Id", "dbo.StoreReceipts");
            DropForeignKey("dbo.InvoiceDetailTemps", "StoreReceiptDetailId", "dbo.StoreReceiptDetails");
            DropForeignKey("dbo.InvoiceDetails", "StoreReceipt_Id", "dbo.StoreReceipts");
            DropForeignKey("dbo.InvoiceDetails", "StoreReceiptDetailId", "dbo.StoreReceiptDetails");
            DropIndex("dbo.InvoiceDetailTemps", new[] { "StoreReceipt_Id" });
            DropIndex("dbo.InvoiceDetailTemps", new[] { "StoreReceiptDetailId" });
            DropIndex("dbo.InvoiceDetails", new[] { "StoreReceipt_Id" });
            DropIndex("dbo.InvoiceDetails", new[] { "StoreReceiptDetailId" });
            DropColumn("dbo.InvoiceDetailTemps", "StoreReceipt_Id");
            DropColumn("dbo.InvoiceDetailTemps", "StoreReceiptDetailId");
            DropColumn("dbo.InvoiceDetails", "StoreReceipt_Id");
            DropColumn("dbo.InvoiceDetails", "StoreReceiptDetailId");
            CreateIndex("dbo.InvoiceDetailTemps", "ProductId");
            CreateIndex("dbo.InvoiceDetails", "ProductId");
            CreateIndex("dbo.Invoices", "StoreReceiptId");
            AddForeignKey("dbo.InvoiceDetailTemps", "ProductId", "dbo.Products", "Id", cascadeDelete: false);
            AddForeignKey("dbo.InvoiceDetails", "ProductId", "dbo.Products", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Invoices", "StoreReceiptId", "dbo.StoreReceipts", "Id", cascadeDelete: false);
        }
    }
}
