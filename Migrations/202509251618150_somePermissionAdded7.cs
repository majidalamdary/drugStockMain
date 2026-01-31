namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models;
    using DrugStockWeb.Models.Account;
    using DrugStockWeb.Models.Constancts;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    
    public partial class somePermissionAdded7 : DbMigration
    {
        public MainDbContext Db = new MainDbContext();
        public override void Up()
        {
            var per = new Permission();
            per.Title = "مشاهده وجود مغایرت در دیتابیس";
            Db.Entry(per).State = EntityState.Added;
            Db.SaveChanges();
        }

        public override void Down()
        {
        }
    }
}
