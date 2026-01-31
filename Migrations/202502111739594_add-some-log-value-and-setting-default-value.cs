using DrugStockWeb.Models.Common;

namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models.LogSystem;
    using DrugStockWeb.Models;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsomelogvalueandsettingdefaultvalue : DbMigration
    {
        public override void Up()
        {
            MainDbContext Db = new MainDbContext();
            LogType logType = new LogType()
            {
                Title = "ورود به سیستم"
            };
            Db.LogTypes.Add(logType);
            logType = new LogType()
            {
                Title = "ایجاد انبار"
            };
            Db.LogTypes.Add(logType);
            logType = new LogType()
            {
                Title = "ویرایش انبار"
            };
            Db.LogTypes.Add(logType);
            logType = new LogType()
            {
                Title = "محدود شدن کابر"
            };

            Db.LogTypes.Add(logType);



            Db.SaveChanges();

        }

        public override void Down()
        {
        }
    }
}
