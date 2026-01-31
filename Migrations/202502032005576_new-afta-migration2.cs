namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models.Constancts;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newaftamigration2 : DbMigration
    {
        public override void Up()
        {
            Sql("insert into UserStatus (Id,Title) values (" + (int)UserStatusValue.Activated + ",N'فعال'),(" + (int)UserStatusValue.UnActivated + ",N'غیر فعال'),(" + (int)UserStatusValue.Blocked + ",N'محدود شده')");
        }
        
        public override void Down()
        {
        }
    }
}
