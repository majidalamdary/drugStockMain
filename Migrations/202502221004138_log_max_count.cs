namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class log_max_count : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Logs", "OldValue", c => c.String());
            AddColumn("dbo.Logs", "NewValue", c => c.String());
            AddColumn("dbo.SecuritySettings", "LogMaximumRecordCount", c => c.Long(nullable: false));
            AddColumn("dbo.SecuritySettings", "LogThresholdCount", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SecuritySettings", "LogThresholdCount");
            DropColumn("dbo.SecuritySettings", "LogMaximumRecordCount");
            DropColumn("dbo.Logs", "NewValue");
            DropColumn("dbo.Logs", "OldValue");
        }
    }
}
