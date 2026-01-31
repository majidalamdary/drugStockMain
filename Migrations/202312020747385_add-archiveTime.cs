namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addarchiveTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinnessPartners", "LastArchivedTime", c => c.DateTime(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinnessPartners", "LastArchivedTime");
        }
    }
}
