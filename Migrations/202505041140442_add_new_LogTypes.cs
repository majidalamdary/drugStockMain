namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models.Constancts;
    using DrugStockWeb.Models.LogSystem;
    using DrugStockWeb.Models;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_new_LogTypes : DbMigration
    {
        public override void Up()
        {
            MainDbContext Db = new MainDbContext();
            LogType logType = logType = new LogType() { Title = "افزودن آی پی غیرمجاز" };
            Db.LogTypes.Add(logType);//105
            Db.SaveChanges();
            logType.HashValue = HashHelper.ComputeSha256Hash(logType, Db);
            Db.LogTypes.AddOrUpdate(logType);
            Db.SaveChanges();
            logType = logType = new LogType() { Title = "حذف آی پی غیرمجاز" };
            Db.LogTypes.Add(logType);//106
            Db.SaveChanges();
            logType.HashValue = HashHelper.ComputeSha256Hash(logType, Db);
            Db.LogTypes.AddOrUpdate(logType);
            Db.SaveChanges();

            //HashHelper.CalculateCommonHash(ModelsNumberValue.LogTypes);
        }
        
        public override void Down()
        {
        }
    }
}
