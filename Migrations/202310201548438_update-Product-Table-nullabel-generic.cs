namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateProductTablenullabelgeneric : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "GenericCode", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "GenericCode", c => c.String(nullable: false));
        }
    }
}
