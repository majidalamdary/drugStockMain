namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_invoice_table5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InvoiceDetails", "StoreReceipt_Id", "dbo.StoreReceipts");
            DropForeignKey("dbo.InvoiceDetailTemps", "StoreReceipt_Id", "dbo.StoreReceipts");
            DropIndex("dbo.InvoiceDetails", new[] { "StoreReceipt_Id" });
            DropIndex("dbo.InvoiceDetailTemps", new[] { "StoreReceipt_Id" });
            DropColumn("dbo.InvoiceDetails", "StoreReceipt_Id");
            DropColumn("dbo.InvoiceDetailTemps", "StoreReceipt_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InvoiceDetailTemps", "StoreReceipt_Id", c => c.Guid());
            AddColumn("dbo.InvoiceDetails", "StoreReceipt_Id", c => c.Guid());
            CreateIndex("dbo.InvoiceDetailTemps", "StoreReceipt_Id");
            CreateIndex("dbo.InvoiceDetails", "StoreReceipt_Id");
            AddForeignKey("dbo.InvoiceDetailTemps", "StoreReceipt_Id", "dbo.StoreReceipts", "Id");
            AddForeignKey("dbo.InvoiceDetails", "StoreReceipt_Id", "dbo.StoreReceipts", "Id");
        }
    }
}
