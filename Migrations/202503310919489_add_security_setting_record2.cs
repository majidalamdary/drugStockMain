using System.Linq;

namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models.Common;
    using DrugStockWeb.Models;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_security_setting_record2 : DbMigration
    {
        public override void Up()
        {
            //MainDbContext Db = new MainDbContext();
            //var model = Db.SecuritySettings.FirstOrDefault();
            //if (model == null)
            //{
                //SecuritySetting securitySetting = new SecuritySetting()
                //{
                //    ActiveUserAfterTimePeriodByMinutes = 15,
                //    FailedLoginMaxTryingTime = 3,
                //    LogMaximumRecordCount = 1000000,
                //    LogThresholdPercentage = 80,
                //    MinPasswordLength = 8
                //};
                //securitySetting.HashValue = HashHelper.ComputeSha256Hash(securitySetting, Db);
                //Db.SecuritySettings.Add(securitySetting);
                //Db.SaveChanges();
            //}
        }
        
        public override void Down()
        {
        }
    }
}
