namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_invoice_table : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Invoices", "FactorNumber", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Invoices", "FactorNumber", c => c.String(nullable: false));
        }
    }
}
