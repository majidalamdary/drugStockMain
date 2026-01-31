namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models;
    using DrugStockWeb.Models.LogSystem;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSomeLogType4 : DbMigration
    {
        public override void Up()
        {
            MainDbContext Db = new MainDbContext();
            LogType logType = logType = new LogType() { Title = "خروج به دلیل تغییر آی پی کاربر" };
            Db.LogTypes.Add(logType);//116
            Db.SaveChanges();
        }
        
        public override void Down()
        {
        }
    }
}
