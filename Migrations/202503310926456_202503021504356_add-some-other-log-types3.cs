namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models.LogSystem;
    using DrugStockWeb.Models;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _202503021504356_addsomeotherlogtypes3 : DbMigration
    {
        public override void Up()
        {
            MainDbContext Db = new MainDbContext();
            LogType logType = logType = new LogType() { Title = "تائید حواله توسط حسابداری" }; Db.LogTypes.Add(logType);//88
            logType = new LogType() { Title = "گزارش کاردکس" }; Db.LogTypes.Add(logType);//89
            logType = new LogType() { Title = "چاپ کاردکس انبار" }; Db.LogTypes.Add(logType);//90
            logType = new LogType() { Title = "تغییرات تنظیمات امنیتی" }; Db.LogTypes.Add(logType);//91
            Db.SaveChanges();
        }
        
        public override void Down()
        {
        }
    }
}
