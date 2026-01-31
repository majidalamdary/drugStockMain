namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addInovoiceReturntmpCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoiceDetails", "ReturnTempCount", c => c.Long(nullable: false,defaultValue:0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InvoiceDetails", "ReturnTempCount");
        }
    }
}
