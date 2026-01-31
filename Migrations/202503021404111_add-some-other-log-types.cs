namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models;
    using DrugStockWeb.Models.LogSystem;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsomeotherlogtypes : DbMigration
    {
        public override void Up()
        {
            MainDbContext Db = new MainDbContext();
            LogType logType = logType = new LogType() { Title = "ایجاد جزئیات رسید انبار" }; Db.LogTypes.Add(logType);//73
            logType = new LogType() { Title = "ویرایش جزئیات رسید انبار" }; Db.LogTypes.Add(logType);//74
            logType = new LogType() { Title = "حذف جزئیات رسید انبار" }; Db.LogTypes.Add(logType);//75
            logType = new LogType() { Title = "ایجاد جزئیات رسید مرجوعی" }; Db.LogTypes.Add(logType);//76
            logType = new LogType() { Title = "ویرایش جزئیات رسید مرجوعی" }; Db.LogTypes.Add(logType);//77
            logType = new LogType() { Title = "ایجاد جزئیات حواله انبار" }; Db.LogTypes.Add(logType);//78
            logType = new LogType() { Title = "ویرایش جزئیات حواله انبار" }; Db.LogTypes.Add(logType);//79
            logType = new LogType() { Title = "حذف جزئیات حواله انبار" }; Db.LogTypes.Add(logType);//80
            logType = new LogType() { Title = "ایجاد جزئیات حواله مرجوعی" }; Db.LogTypes.Add(logType);//81
            logType = new LogType() { Title = "ویرایش جزئیات حواله مرجوعی" }; Db.LogTypes.Add(logType);//82
            logType = new LogType() { Title = "حذف جزئیات حواله مرجوعی" }; Db.LogTypes.Add(logType);//83
            Db.SaveChanges();

        }

        public override void Down()
        {
        }
    }
}
