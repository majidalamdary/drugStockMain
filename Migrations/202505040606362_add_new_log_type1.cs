namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models.Constancts;
    using DrugStockWeb.Models.LogSystem;
    using DrugStockWeb.Models;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_new_log_type1 : DbMigration
    {
        public override void Up()
        {
            MainDbContext Db = new MainDbContext();
            LogType logType = logType = new LogType() { Title = "خطای ناشناخته" };
            Db.LogTypes.Add(logType);//104
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
