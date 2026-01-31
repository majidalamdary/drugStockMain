namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addStoreReceipdetailreturnTempCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreReceiptDetails", "ReturnTempCount", c => c.Long(nullable:true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoreReceiptDetails", "ReturnTempCount");
        }
    }
}
