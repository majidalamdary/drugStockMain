namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models;
    using DrugStockWeb.Models.LogSystem;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class some_logs_type_added : DbMigration
    {
        public override void Up()
        {
            MainDbContext Db = new MainDbContext();



        }

        public override void Down()
        {
        }
    }
}
