namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SyncDatabaseWithModelsAndLogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LogDateTime = c.DateTime(nullable: false),
                        LogTypeId = c.Int(nullable: false),
                        Creator = c.String(nullable: false),
                        LogStatus = c.Boolean(nullable: false),
                        IPAddress = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LogTypes", t => t.LogTypeId, cascadeDelete: true)
                .Index(t => t.LogTypeId);
            
            CreateTable(
                "dbo.LogTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            //DropColumn("dbo.Cities", "AmarCode");
            //DropColumn("dbo.Provinces", "AmarCode");
            //DropColumn("dbo.Provinces", "Description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Provinces", "Description", c => c.String());
            AddColumn("dbo.Provinces", "AmarCode", c => c.Int(nullable: false));
            AddColumn("dbo.Cities", "AmarCode", c => c.Int(nullable: false));
            DropForeignKey("dbo.Logs", "LogTypeId", "dbo.LogTypes");
            DropIndex("dbo.Logs", new[] { "LogTypeId" });
            DropTable("dbo.LogTypes");
            DropTable("dbo.Logs");
        }
    }
}
