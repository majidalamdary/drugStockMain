namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addhashfields2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GenralHashes", "LogModel", c => c.Binary());
            AddColumn("dbo.GenralHashes", "LogSettingModel", c => c.Binary());
            AddColumn("dbo.GenralHashes", "LogType", c => c.Binary());
            AddColumn("dbo.Logs", "HashValue", c => c.Binary());
            AddColumn("dbo.LogTypes", "HashValue", c => c.Binary());
            AddColumn("dbo.LogSettings", "HashValue", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LogSettings", "HashValue");
            DropColumn("dbo.LogTypes", "HashValue");
            DropColumn("dbo.Logs", "HashValue");
            DropColumn("dbo.GenralHashes", "LogType");
            DropColumn("dbo.GenralHashes", "LogSettingModel");
            DropColumn("dbo.GenralHashes", "LogModel");
        }
    }
}
