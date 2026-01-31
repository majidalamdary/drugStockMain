namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_is_seen_to_log : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Logs", "IsSeen", c => c.Boolean(nullable: false,defaultValue:false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Logs", "IsSeen");
        }
    }
}
