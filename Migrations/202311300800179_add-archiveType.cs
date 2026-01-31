using DrugStockWeb.Models.BusinessPartner;

namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addarchiveType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinnessPartners", "IsArchived", c => c.Boolean(nullable: false,defaultValue:false));
            AddColumn("dbo.BusinnessPartners", "ArchiveTypeId", c => c.Int(nullable: false,defaultValue:(int?)ArchiveType.Active));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinnessPartners", "ArchiveTypeId");
            DropColumn("dbo.BusinnessPartners", "IsArchived");
        }
    }
}
