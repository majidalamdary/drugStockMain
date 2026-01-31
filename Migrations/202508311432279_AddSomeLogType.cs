namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models;
    using DrugStockWeb.Models.LogSystem;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSomeLogType : DbMigration
    {
        public override void Up()
        {
            MainDbContext Db = new MainDbContext();
            //LogType logType = logType = new LogType() { Title = "مشاهده هشدار عبور از حد آستانه توسط مدیر" };
            //Db.LogTypes.Add(logType);//108
            //Db.SaveChanges();
            LogType logType = logType = new LogType() { Title = " عبور تعداد لاگ ها از حداکثر " };
            Db.LogTypes.Add(logType);//110
            Db.SaveChanges();
            logType = logType = new LogType() { Title = " مشاهده هشدار عبور تعداد لاگ ها از حداکثر " };
            Db.LogTypes.Add(logType);//111
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
