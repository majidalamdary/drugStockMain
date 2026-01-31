namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_store_to_invoice_and_receipt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "StoreId", c => c.Guid(nullable: false));
            AddColumn("dbo.StoreReceipts", "StoreId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Invoices", "StoreId");
            CreateIndex("dbo.StoreReceipts", "StoreId");
            AddForeignKey("dbo.Invoices", "StoreId", "dbo.Stores", "Id", cascadeDelete: false);
            AddForeignKey("dbo.StoreReceipts", "StoreId", "dbo.Stores", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoreReceipts", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.Invoices", "StoreId", "dbo.Stores");
            DropIndex("dbo.StoreReceipts", new[] { "StoreId" });
            DropIndex("dbo.Invoices", new[] { "StoreId" });
            DropColumn("dbo.StoreReceipts", "StoreId");
            DropColumn("dbo.Invoices", "StoreId");
        }
    }
}
