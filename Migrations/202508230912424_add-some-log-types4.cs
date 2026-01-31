namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models;
    using DrugStockWeb.Models.Constancts;
    using DrugStockWeb.Models.LogSystem;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsomelogtypes4 : DbMigration
    {
        public override void Up()
        {
            MainDbContext Db = new MainDbContext();
            //LogType logType = logType = new LogType() { Title = "مشاهده هشدار عبور از حد آستانه توسط مدیر" };
            //Db.LogTypes.Add(logType);//108
            //Db.SaveChanges();
            LogType logType = logType = new LogType() { Title = " هشدار دستکاری دیتابیس " };
            Db.LogTypes.Add(logType);//108
            Db.SaveChanges();
            logType = logType = new LogType() { Title = "مشاهده هشدار دستکاری دیتابیس توسط مجاز" };
            Db.LogTypes.Add(logType);//109
            Db.SaveChanges();

            logType.HashValue = HashHelper.ComputeSha256Hash(logType, Db);
            Db.LogTypes.AddOrUpdate(logType);

            Db.SaveChanges();
    
        }
        
        public override void Down()
        {
        }
    }
}
