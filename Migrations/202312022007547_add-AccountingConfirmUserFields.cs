namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAccountingConfirmUserFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "AccountingConfirmerUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Invoices", "AccountingConfirmerUserId");
            AddForeignKey("dbo.Invoices", "AccountingConfirmerUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Invoices", "AccountingConfirmerUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Invoices", new[] { "AccountingConfirmerUserId" });
            DropColumn("dbo.Invoices", "AccountingConfirmerUserId");
        }
    }
}
