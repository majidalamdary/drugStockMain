namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addeconomicalcode : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.BusinnessPartners", "EconomicalCode", c => c.String());
        }
        
        public override void Down()
        {
            //DropColumn("dbo.BusinnessPartners", "EconomicalCode");
        }
    }
}
