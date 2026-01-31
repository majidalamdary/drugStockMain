using DrugStockWeb.Models.Account;

namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models.Constancts;
    using DrugStockWeb.Models.LogSystem;
    using DrugStockWeb.Models;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_new_LogTypes3 : DbMigration
    {
        public override void Up()
        {
            MainDbContext Db = new MainDbContext();
            LogType logType = logType = new LogType() { Title = "مشاهده آی پی غیرمجاز" };
            Db.LogTypes.Add(logType);//107
            Db.SaveChanges();
            logType.HashValue = HashHelper.ComputeSha256Hash(logType, Db);
            Db.LogTypes.AddOrUpdate(logType);
            
            var per = new Permission()//57
            {
                Title = "مشاهده آی پی های غیرمجاز"
            };
            Db.Permissions.Add(per);

            per = new Permission("افزودن آی پی های غیرمجاز");//58
            Db.Permissions.Add(per);
            per = new Permission("حذف آی پی های غیرمجاز");//59
            Db.Permissions.Add(per);
            Db.SaveChanges();
            //HashHelper.CalculateCommonHash(ModelsNumberValue.LogTypes);
        }
        
        public override void Down()
        {
        }
    }
}
