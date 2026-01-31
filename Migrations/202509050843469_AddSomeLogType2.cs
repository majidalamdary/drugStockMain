namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models;
    using DrugStockWeb.Models.LogSystem;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSomeLogType2 : DbMigration
    {
        public override void Up()
        {
            MainDbContext Db = new MainDbContext();
            LogType logType = logType = new LogType() { Title = "خروج کاربر به دلیل غیر فعال بودن" };
            Db.LogTypes.Add(logType);//112
            Db.SaveChanges();
            logType = logType = new LogType() { Title = "خروج کاربر به دلیل تغییرات مشخصات" };
            Db.LogTypes.Add(logType);//113
            Db.SaveChanges();
            logType = logType = new LogType() { Title = "خروج کاربر به دلیل نشست همزمان" };
            Db.LogTypes.Add(logType);//114
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
