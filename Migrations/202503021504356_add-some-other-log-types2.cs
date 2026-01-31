namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models.LogSystem;
    using DrugStockWeb.Models;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsomeotherlogtypes2 : DbMigration
    {
        public override void Up()
        {
            MainDbContext Db = new MainDbContext();
            LogType logType = logType = new LogType() { Title = "چاپ گزارش رسید انبار" }; Db.LogTypes.Add(logType);//84
            logType = new LogType() { Title = "چاپ گزارس حواله انبار" }; Db.LogTypes.Add(logType);//85
            logType = new LogType() { Title = "چاپ موجودی انبار" }; Db.LogTypes.Add(logType);//86
            logType = new LogType() { Title = "چاپ موجودی انبار امحا" }; Db.LogTypes.Add(logType);//87
            Db.SaveChanges();
        }
        
        public override void Down()
        {
        }
    }
}
