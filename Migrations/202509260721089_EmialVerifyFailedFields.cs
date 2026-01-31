namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmialVerifyFailedFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FailedEmailVerifyTryTimes", c => c.Byte(defaultValue:0));
            AddColumn("dbo.AspNetUsers", "FailedEmailVerifyDateTime", c => c.DateTime());
            Sql("UPDATE dbo.AspNetUsers SET FailedEmailVerifyTryTimes = 0, FailedEmailVerifyDateTime = NULL");

        }

        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "FailedEmailVerifyDateTime");
            DropColumn("dbo.AspNetUsers", "FailedEmailVerifyTryTimes");
        }
    }
}
