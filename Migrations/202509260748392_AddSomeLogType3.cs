namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models;
    using DrugStockWeb.Models.LogSystem;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSomeLogType3 : DbMigration
    {
        public override void Up()
        {
            MainDbContext Db = new MainDbContext();
            LogType logType = logType = new LogType() { Title = "ورود ناموفق کد دو مرحله ای" };
            Db.LogTypes.Add(logType);//115
            Db.SaveChanges();
        }
        
        public override void Down()
        {
        }
    }
}
