namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatestoreReceipttable4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreReceiptDetails", "StoreReceiptId", c => c.Guid(nullable: false));
            CreateIndex("dbo.StoreReceiptDetails", "StoreReceiptId");
            AddForeignKey("dbo.StoreReceiptDetails", "StoreReceiptId", "dbo.StoreReceipts", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoreReceiptDetails", "StoreReceiptId", "dbo.StoreReceipts");
            DropIndex("dbo.StoreReceiptDetails", new[] { "StoreReceiptId" });
            DropColumn("dbo.StoreReceiptDetails", "StoreReceiptId");
        }
    }
}
