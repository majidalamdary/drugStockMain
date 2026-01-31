namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addStoreReceiptTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StoreReceipts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ReceiptNumber = c.Long(nullable: false),
                        Title = c.String(nullable: false),
                        RequestNumber = c.String(nullable: false),
                        FactorNumber = c.String(nullable: false),
                        FactorDate = c.DateTime(nullable: false),
                        ReceiptDate = c.DateTime(nullable: false),
                        BusinnessPartnerId = c.Guid(nullable: false),
                        CreatorUserId = c.String(maxLength: 128),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BusinnessPartners", t => t.BusinnessPartnerId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorUserId)
                .Index(t => t.BusinnessPartnerId)
                .Index(t => t.CreatorUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoreReceipts", "CreatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.StoreReceipts", "BusinnessPartnerId", "dbo.BusinnessPartners");
            DropIndex("dbo.StoreReceipts", new[] { "CreatorUserId" });
            DropIndex("dbo.StoreReceipts", new[] { "BusinnessPartnerId" });
            DropTable("dbo.StoreReceipts");
        }
    }
}
