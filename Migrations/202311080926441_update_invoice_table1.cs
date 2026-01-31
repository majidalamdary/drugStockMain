namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_invoice_table1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoiceDetailTemps", "InvoiceId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InvoiceDetailTemps", "InvoiceId");
        }
    }
}
