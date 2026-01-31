namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addManufactureTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Manufactures",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(nullable: false),
                        CreatorUserId = c.String(maxLength: 128),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorUserId)
                .Index(t => t.CreatorUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Manufactures", "CreatorUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Manufactures", new[] { "CreatorUserId" });
            DropTable("dbo.Manufactures");
        }
    }
}
