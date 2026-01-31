namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models.LogSystem;
    using DrugStockWeb.Models;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _202503021504356_addsomeotherlogtypes4 : DbMigration
    {
        public override void Up()
        {
            MainDbContext Db = new MainDbContext();
            LogType logType = logType = new LogType() { Title = "جدول لاگ ها حذف شده" }; Db.LogTypes.Add(logType);//94
            Db.SaveChanges();
        }
        
        public override void Down()
        {
        }
    }
}
