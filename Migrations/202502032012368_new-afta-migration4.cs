namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models.Constancts;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newaftamigration4 : DbMigration
    {
        public override void Up()
        {
            Sql("update AspNetUsers set UserStatusId="+(int)UserStatusValue.Activated);
        }
        
        public override void Down()
        {
        }
    }
}
