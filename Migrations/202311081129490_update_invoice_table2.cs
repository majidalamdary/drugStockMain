namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_invoice_table2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "ReceiverFullName", c => c.String());
            AddColumn("dbo.Invoices", "ReceiverMobile", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoices", "ReceiverMobile");
            DropColumn("dbo.Invoices", "ReceiverFullName");
        }
    }
}
