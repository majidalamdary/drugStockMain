namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatebusinnesspartner : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinnessPartnerGroups", "SellWithBuyPrice", c => c.Boolean(nullable: false));
            AddColumn("dbo.BusinnessPartnerGroups", "CreatorUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.BusinnessPartnerGroups", "CreateDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.BusinnessPartnerGroups", "UpdateDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.BusinnessPartnerGroups", "CreatorUserId");
            AddForeignKey("dbo.BusinnessPartnerGroups", "CreatorUserId", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.BusinnessPartnerGroups", "SellPrice");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BusinnessPartnerGroups", "SellPrice", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.BusinnessPartnerGroups", "CreatorUserId", "dbo.AspNetUsers");
            DropIndex("dbo.BusinnessPartnerGroups", new[] { "CreatorUserId" });
            DropColumn("dbo.BusinnessPartnerGroups", "UpdateDate");
            DropColumn("dbo.BusinnessPartnerGroups", "CreateDate");
            DropColumn("dbo.BusinnessPartnerGroups", "CreatorUserId");
            DropColumn("dbo.BusinnessPartnerGroups", "SellWithBuyPrice");
        }
    }
}
