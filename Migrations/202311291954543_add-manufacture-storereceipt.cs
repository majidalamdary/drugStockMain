namespace DrugStockWeb.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class addmanufacturestorereceipt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreReceiptDetails", "ManufactureId", c => c.Guid());
            CreateIndex("dbo.StoreReceiptDetails", "ManufactureId");
            AddForeignKey("dbo.StoreReceiptDetails", "ManufactureId", "dbo.Manufactures", "Id");
        }
        public override void Down()
        {
            DropForeignKey("dbo.StoreReceiptDetails", "ManufactureId", "dbo.Manufactures");
            DropIndex("dbo.StoreReceiptDetails", new[] { "ManufactureId" });
            DropColumn("dbo.StoreReceiptDetails", "ManufactureId");
        }
    }
}
