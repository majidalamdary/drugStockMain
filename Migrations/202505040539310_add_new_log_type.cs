namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models.LogSystem;
    using DrugStockWeb.Models;
    using System;
    using System.Data.Entity.Migrations;
    using DrugStockWeb.Models.Constancts;

    public partial class add_new_log_type : DbMigration
    {
        public override void Up()
        {
            MainDbContext Db = new MainDbContext();
            LogType logType = logType = new LogType() { Title = "ورود داده غیر مجاز توسط کاربر" }; 
            Db.LogTypes.Add(logType);//103
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
