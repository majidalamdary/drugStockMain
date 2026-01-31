namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change_manufacture_id_field : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Products", "Manufacture_Id", "dbo.Manufactures");
            DropIndex("dbo.Products", new[] { "Manufacture_Id" });
            DropColumn("dbo.Products", "ManufactureId");
            RenameColumn(table: "dbo.Products", name: "Manufacture_Id", newName: "ManufactureId");
            AlterColumn("dbo.Products", "ManufactureId", c => c.Guid(nullable: false));
            AlterColumn("dbo.Products", "ManufactureId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Products", "ManufactureId");
            AddForeignKey("dbo.Products", "ManufactureId", "dbo.Manufactures", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "ManufactureId", "dbo.Manufactures");
            DropIndex("dbo.Products", new[] { "ManufactureId" });
            AlterColumn("dbo.Products", "ManufactureId", c => c.Guid());
            AlterColumn("dbo.Products", "ManufactureId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Products", name: "ManufactureId", newName: "Manufacture_Id");
            AddColumn("dbo.Products", "ManufactureId", c => c.Int(nullable: false));
            CreateIndex("dbo.Products", "Manufacture_Id");
            AddForeignKey("dbo.Products", "Manufacture_Id", "dbo.Manufactures", "Id");
        }
    }
}
