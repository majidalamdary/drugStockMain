namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addbusinnesspartner : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusinnessPartnerGroups",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(nullable: false),
                        SellPrice = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Products", "ManufactureId", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "Manufacture_Id", c => c.Guid());
            CreateIndex("dbo.Products", "Manufacture_Id");
            AddForeignKey("dbo.Products", "Manufacture_Id", "dbo.Manufactures", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "Manufacture_Id", "dbo.Manufactures");
            DropIndex("dbo.Products", new[] { "Manufacture_Id" });
            DropColumn("dbo.Products", "Manufacture_Id");
            DropColumn("dbo.Products", "ManufactureId");
            DropTable("dbo.BusinnessPartnerGroups");
        }
    }
}
