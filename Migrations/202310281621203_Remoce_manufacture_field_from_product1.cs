namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remoce_manufacture_field_from_product1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Products", "Manufacture_Id", "dbo.Manufactures");
            DropIndex("dbo.Products", new[] { "Manufacture_Id" });
            DropColumn("dbo.Products", "Manufacture_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "Manufacture_Id", c => c.Guid());
            CreateIndex("dbo.Products", "Manufacture_Id");
            AddForeignKey("dbo.Products", "Manufacture_Id", "dbo.Manufactures", "Id");
        }
    }
}
