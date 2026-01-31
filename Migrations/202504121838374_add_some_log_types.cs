namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models.LogSystem;
    using DrugStockWeb.Models;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_some_log_types : DbMigration
    {
        public override void Up()
        {
            MainDbContext Db = new MainDbContext();
            LogType logType = logType = new LogType() { Title = "اضافه کردن نقش به کاربر" }; Db.LogTypes.Add(logType);//93
            logType = new LogType() { Title = "تغییر نقش کاربر" }; Db.LogTypes.Add(logType);//94
            logType = new LogType() { Title = "اضافه کردن انبار به کاربر" }; Db.LogTypes.Add(logType);//95
            logType = new LogType() { Title = "حذف انبار از کاربر" }; Db.LogTypes.Add(logType);//96
            logType = new LogType() { Title = "خروج کاربر" }; Db.LogTypes.Add(logType);//97
            logType = new LogType() { Title = "غیرفعال کرن کاربر توسط مدیر" }; Db.LogTypes.Add(logType);//98
            logType = new LogType() { Title = "فعال کرن کاربر توسط مدیر" }; Db.LogTypes.Add(logType);//99
            logType = new LogType() { Title = "خروج کاربر توسط مدیر" }; Db.LogTypes.Add(logType);//100
            logType = new LogType() { Title = "لیست کاربران آنلاین" }; Db.LogTypes.Add(logType);//101
            Db.SaveChanges();
        }
        
        public override void Down()
        {
        }
    }
}
