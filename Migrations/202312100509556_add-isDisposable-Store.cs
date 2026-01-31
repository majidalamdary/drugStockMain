namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addisDisposableStore : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stores", "IsForDisposable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stores", "IsForDisposable");
        }
    }
}
