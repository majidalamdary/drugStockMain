using System.Linq;

namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models.Common;
    using DrugStockWeb.Models;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_security_setting_record : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SecuritySettings", "LogThresholdPercentage", c => c.Long(nullable: false));
            DropColumn("dbo.SecuritySettings", "LogThresholdCount");
            
        }
        
        public override void Down()
        {
            AddColumn("dbo.SecuritySettings", "LogThresholdCount", c => c.Long(nullable: false));
            DropColumn("dbo.SecuritySettings", "LogThresholdPercentage");
        }
    }
}
