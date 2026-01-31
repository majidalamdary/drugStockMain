namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addInovoiceReturn : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InvoiceDetailReturns",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        InvoiceDetailId = c.Guid(nullable: false),
                        InvoiceReturnId = c.Guid(nullable: false),
                        Count = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InvoiceDetails", t => t.InvoiceDetailId, cascadeDelete: false)
                .ForeignKey("dbo.InvoiceReturns", t => t.InvoiceReturnId, cascadeDelete: false)
                .Index(t => t.InvoiceDetailId)
                .Index(t => t.InvoiceReturnId);
            
            CreateTable(
                "dbo.InvoiceReturns",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        InvoiceReturnDate = c.DateTime(nullable: false),
                        IsConfirmed = c.Boolean(nullable: false),
                        Describ = c.String(),
                        CreatorUserId = c.String(maxLength: 128),
                        ConfirmerUserId = c.String(maxLength: 128),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                        IsDisposed = c.Boolean(nullable: false),
                        StoreId = c.Guid(nullable: false),
                        InvoiceId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ConfirmerUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorUserId)
                .ForeignKey("dbo.Invoices", t => t.InvoiceId, cascadeDelete: false)
                .ForeignKey("dbo.Stores", t => t.StoreId, cascadeDelete: false)
                .Index(t => t.CreatorUserId)
                .Index(t => t.ConfirmerUserId)
                .Index(t => t.StoreId)
                .Index(t => t.InvoiceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InvoiceReturns", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.InvoiceDetailReturns", "InvoiceReturnId", "dbo.InvoiceReturns");
            DropForeignKey("dbo.InvoiceReturns", "InvoiceId", "dbo.Invoices");
            DropForeignKey("dbo.InvoiceReturns", "CreatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.InvoiceReturns", "ConfirmerUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.InvoiceDetailReturns", "InvoiceDetailId", "dbo.InvoiceDetails");
            DropIndex("dbo.InvoiceReturns", new[] { "InvoiceId" });
            DropIndex("dbo.InvoiceReturns", new[] { "StoreId" });
            DropIndex("dbo.InvoiceReturns", new[] { "ConfirmerUserId" });
            DropIndex("dbo.InvoiceReturns", new[] { "CreatorUserId" });
            DropIndex("dbo.InvoiceDetailReturns", new[] { "InvoiceReturnId" });
            DropIndex("dbo.InvoiceDetailReturns", new[] { "InvoiceDetailId" });
            DropTable("dbo.InvoiceReturns");
            DropTable("dbo.InvoiceDetailReturns");
        }
    }
}
