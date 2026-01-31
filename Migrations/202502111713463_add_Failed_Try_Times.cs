namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_Failed_Try_Times : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FailedTryTimes", c => c.Byte(nullable: false, defaultValue: 0));

            // Update existing records to have a default value of 0
            Sql("UPDATE dbo.AspNetUsers SET FailedTryTimes = 0");
        }

        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "FailedTryTimes");
        }

    }
}
