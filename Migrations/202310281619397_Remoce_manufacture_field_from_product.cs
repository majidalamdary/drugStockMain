namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remoce_manufacture_field_from_product : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Products", "ManufactureId", "dbo.Manufactures");
            DropIndex("dbo.Products", new[] { "ManufactureId" });
            RenameColumn(table: "dbo.Products", name: "ManufactureId", newName: "Manufacture_Id");
            AlterColumn("dbo.Products", "Manufacture_Id", c => c.Guid());
            CreateIndex("dbo.Products", "Manufacture_Id");
            AddForeignKey("dbo.Products", "Manufacture_Id", "dbo.Manufactures", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "Manufacture_Id", "dbo.Manufactures");
            DropIndex("dbo.Products", new[] { "Manufacture_Id" });
            AlterColumn("dbo.Products", "Manufacture_Id", c => c.Guid(nullable: false));
            RenameColumn(table: "dbo.Products", name: "Manufacture_Id", newName: "ManufactureId");
            CreateIndex("dbo.Products", "ManufactureId");
            AddForeignKey("dbo.Products", "ManufactureId", "dbo.Manufactures", "Id", cascadeDelete: true);
        }
    }
}
