namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateBusinnessPartnerGroupTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinnessPartnerGroups", "BusinnessPartnerStatusId", c => c.Int(nullable: false));
            AddColumn("dbo.StoreReceipts", "Describ", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoreReceipts", "Describ");
            DropColumn("dbo.BusinnessPartnerGroups", "BusinnessPartnerStatusId");
        }
    }
}
