namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models.LogSystem;
    using DrugStockWeb.Models;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_some_log_types2 : DbMigration
    {
        public override void Up()
        {
            MainDbContext Db = new MainDbContext();
            LogType logType = logType = new LogType() { Title = "عبور از حد آستانه تعداد لاگ" }; Db.LogTypes.Add(logType);//102
            Db.SaveChanges();
        }
        
        public override void Down()
        {
        }
    }
}
