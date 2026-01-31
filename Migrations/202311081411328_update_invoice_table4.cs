namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_invoice_table4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Invoices", "Invoice_Id", "dbo.Invoices");
            DropIndex("dbo.Invoices", new[] { "Invoice_Id" });
            DropColumn("dbo.Invoices", "Invoice_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Invoices", "Invoice_Id", c => c.Guid());
            CreateIndex("dbo.Invoices", "Invoice_Id");
            AddForeignKey("dbo.Invoices", "Invoice_Id", "dbo.Invoices", "Id");
        }
    }
}
