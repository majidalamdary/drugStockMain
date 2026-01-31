namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class set_creator_nullable_in_log : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Logs", "Creator", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Logs", "Creator", c => c.String(nullable: false));
        }
    }
}
