namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models;
    using DrugStockWeb.Models.Common;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            MainDbContext Db = new MainDbContext();
            SecuritySetting logSetting = new SecuritySetting()
            {
                ActiveUserAfterTimePeriodByMinutes = 5,
                FailedLoginMaxTryingTime = 3,
                MinPasswordLength = 8,

            };
            logSetting.HashValue = HashHelper.ComputeSha256Hash(logSetting, Db);
            Db.SecuritySettings.Add(logSetting);
        }
        
        public override void Down()
        {
        }
    }
}
