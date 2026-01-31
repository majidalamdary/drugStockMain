namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateBusinnessPartnerTable2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BusinnessPartners", "BusinnessPartnerLegalType_Id", "dbo.BusinnessPartnerLegalTypes");
            DropIndex("dbo.BusinnessPartners", new[] { "BusinnessPartnerLegalType_Id" });
            DropColumn("dbo.BusinnessPartners", "BusinnessPartnerLegalTypeId");
            RenameColumn(table: "dbo.BusinnessPartners", name: "BusinnessPartnerLegalType_Id", newName: "BusinnessPartnerLegalTypeId");
            AlterColumn("dbo.BusinnessPartners", "BusinnessPartnerLegalTypeId", c => c.Int(nullable: false));
            AlterColumn("dbo.BusinnessPartners", "BusinnessPartnerLegalTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.BusinnessPartners", "BusinnessPartnerLegalTypeId");
            AddForeignKey("dbo.BusinnessPartners", "BusinnessPartnerLegalTypeId", "dbo.BusinnessPartnerLegalTypes", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BusinnessPartners", "BusinnessPartnerLegalTypeId", "dbo.BusinnessPartnerLegalTypes");
            DropIndex("dbo.BusinnessPartners", new[] { "BusinnessPartnerLegalTypeId" });
            AlterColumn("dbo.BusinnessPartners", "BusinnessPartnerLegalTypeId", c => c.Int());
            AlterColumn("dbo.BusinnessPartners", "BusinnessPartnerLegalTypeId", c => c.Guid(nullable: false));
            RenameColumn(table: "dbo.BusinnessPartners", name: "BusinnessPartnerLegalTypeId", newName: "BusinnessPartnerLegalType_Id");
            AddColumn("dbo.BusinnessPartners", "BusinnessPartnerLegalTypeId", c => c.Guid(nullable: false));
            CreateIndex("dbo.BusinnessPartners", "BusinnessPartnerLegalType_Id");
            AddForeignKey("dbo.BusinnessPartners", "BusinnessPartnerLegalType_Id", "dbo.BusinnessPartnerLegalTypes", "Id");
        }
    }
}
