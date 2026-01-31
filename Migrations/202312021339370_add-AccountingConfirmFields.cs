namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAccountingConfirmFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "IsAccountingConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Invoices", "AccountingConfirmTime", c => c.DateTime(nullable:true));
            AlterColumn("dbo.BusinnessPartners", "LastArchivedTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BusinnessPartners", "LastArchivedTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.Invoices", "AccountingConfirmTime");
            DropColumn("dbo.Invoices", "IsAccountingConfirmed");
        }
    }
}
