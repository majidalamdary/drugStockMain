namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models;
    using DrugStockWeb.Models.Account;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class somePermissionAdded2 : DbMigration
    {
        public MainDbContext Db = new MainDbContext();

        public override void Up()
        {
            AddColumn("dbo.InvoiceReturns", "DisposerUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.InvoiceReturns", "DisposerUserId");
            AddForeignKey("dbo.InvoiceReturns", "DisposerUserId", "dbo.AspNetUsers", "Id");
  

            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InvoiceReturns", "DisposerUserId", "dbo.AspNetUsers");
            DropIndex("dbo.InvoiceReturns", new[] { "DisposerUserId" });
            DropColumn("dbo.InvoiceReturns", "DisposerUserId");
        }
    }
}
