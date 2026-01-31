using System.Data.Entity;
using DrugStockWeb.Models.Constancts;

namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models;
    using DrugStockWeb.Models.Account;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class somePermissionAdded6 : DbMigration
    {
        public MainDbContext Db = new MainDbContext();
        public override void Up()
        {
            var per = Db.Permissions.Find(PermissionValue.ShowSecuritySetting);
            per.Title = "نمایش تنظیمات امنیتی";
            Db.Entry(per).State = EntityState.Modified;
            per = new Permission()//60
            {
                Title = "ویرایش تنظیمات امنیتی"
            };
            Db.Permissions.Add(per);
            Db.SaveChanges();
        }
        public override void Down()
        {
        }
    }
}
