namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCodeToProductGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductGroups", "Code", c => c.Int(nullable: false));
            AddColumn("dbo.ProductSubGroups", "Code", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductSubGroups", "Code");
            DropColumn("dbo.ProductGroups", "Code");
        }
    }
}
