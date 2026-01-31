namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addusageperiodtoinvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "UsagePeriod", c => c.Long(nullable: false,defaultValue:0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoices", "UsagePeriod");
        }
    }
}
