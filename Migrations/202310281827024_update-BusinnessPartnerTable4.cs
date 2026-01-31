namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateBusinnessPartnerTable4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BusinnessPartners", "BusinnessPartnerLegalTypeId", "dbo.BusinnessPartnerLegalTypes");
            DropIndex("dbo.BusinnessPartners", new[] { "BusinnessPartnerLegalTypeId" });
            AddColumn("dbo.BusinnessPartnerGroups", "BusinnessPartnerLegalTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.BusinnessPartnerGroups", "BusinnessPartnerLegalTypeId");
            AddForeignKey("dbo.BusinnessPartnerGroups", "BusinnessPartnerLegalTypeId", "dbo.BusinnessPartnerLegalTypes", "Id", cascadeDelete: false);
            DropColumn("dbo.BusinnessPartners", "BusinnessPartnerLegalTypeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BusinnessPartners", "BusinnessPartnerLegalTypeId", c => c.Int(nullable: false));
            DropForeignKey("dbo.BusinnessPartnerGroups", "BusinnessPartnerLegalTypeId", "dbo.BusinnessPartnerLegalTypes");
            DropIndex("dbo.BusinnessPartnerGroups", new[] { "BusinnessPartnerLegalTypeId" });
            DropColumn("dbo.BusinnessPartnerGroups", "BusinnessPartnerLegalTypeId");
            CreateIndex("dbo.BusinnessPartners", "BusinnessPartnerLegalTypeId");
            AddForeignKey("dbo.BusinnessPartners", "BusinnessPartnerLegalTypeId", "dbo.BusinnessPartnerLegalTypes", "Id", cascadeDelete: false);
        }
    }
}
