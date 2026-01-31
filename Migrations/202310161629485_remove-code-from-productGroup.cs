namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removecodefromproductGroup : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ProductGroups", "Code");
            DropColumn("dbo.ProductSubGroups", "Code");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductSubGroups", "Code", c => c.Int(nullable: false));
            AddColumn("dbo.ProductGroups", "Code", c => c.Int(nullable: false));
        }
    }
}
