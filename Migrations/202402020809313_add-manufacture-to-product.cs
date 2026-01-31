namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmanufacturetoproduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ManufactureId", c => c.Guid(nullable:true));
            CreateIndex("dbo.Products", "ManufactureId");
            AddForeignKey("dbo.Products", "ManufactureId", "dbo.Manufactures", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "ManufactureId", "dbo.Manufactures");
            DropIndex("dbo.Products", new[] { "ManufactureId" });
            DropColumn("dbo.Products", "ManufactureId");
        }
    }
}
