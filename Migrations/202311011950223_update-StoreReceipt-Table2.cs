namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateStoreReceiptTable2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreReceipts", "IsConfirmed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoreReceipts", "IsConfirmed");
        }
    }
}
