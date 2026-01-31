namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adddescription_to_Province : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Provinces", "Description", c => c.String(defaultValue:"1"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Provinces", "Description");
        }
    }
}
