namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateBusinnessPartnerTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinnessPartners", "BusinnessPartnerGroupId", c => c.Guid(nullable: false));
            AddColumn("dbo.BusinnessPartners", "BusinnessPartnerLegalTypeId", c => c.Guid(nullable: false));
            AddColumn("dbo.BusinnessPartners", "BusinnessPartnerLegalType_Id", c => c.Int());
            CreateIndex("dbo.BusinnessPartners", "BusinnessPartnerGroupId");
            CreateIndex("dbo.BusinnessPartners", "BusinnessPartnerLegalType_Id");
            AddForeignKey("dbo.BusinnessPartners", "BusinnessPartnerGroupId", "dbo.BusinnessPartnerGroups", "Id", cascadeDelete: false);
            AddForeignKey("dbo.BusinnessPartners", "BusinnessPartnerLegalType_Id", "dbo.BusinnessPartnerLegalTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BusinnessPartners", "BusinnessPartnerLegalType_Id", "dbo.BusinnessPartnerLegalTypes");
            DropForeignKey("dbo.BusinnessPartners", "BusinnessPartnerGroupId", "dbo.BusinnessPartnerGroups");
            DropIndex("dbo.BusinnessPartners", new[] { "BusinnessPartnerLegalType_Id" });
            DropIndex("dbo.BusinnessPartners", new[] { "BusinnessPartnerGroupId" });
            DropColumn("dbo.BusinnessPartners", "BusinnessPartnerLegalType_Id");
            DropColumn("dbo.BusinnessPartners", "BusinnessPartnerLegalTypeId");
            DropColumn("dbo.BusinnessPartners", "BusinnessPartnerGroupId");
        }
    }
}
