namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmanufacturestorereceiptTemp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreReceiptDetailTemps", "ManufactureId", c => c.Guid());
            CreateIndex("dbo.StoreReceiptDetailTemps", "ManufactureId");
            AddForeignKey("dbo.StoreReceiptDetailTemps", "ManufactureId", "dbo.Manufactures", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoreReceiptDetailTemps", "ManufactureId", "dbo.Manufactures");
            DropIndex("dbo.StoreReceiptDetailTemps", new[] { "ManufactureId" });
            DropColumn("dbo.StoreReceiptDetailTemps", "ManufactureId");
        }
    }
}
