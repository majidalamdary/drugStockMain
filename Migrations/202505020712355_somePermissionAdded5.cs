namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models;
    using DrugStockWeb.Models.Account;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class somePermissionAdded5 : DbMigration
    {
        public MainDbContext Db = new MainDbContext();
        public override void Up()
        {
            var per = new Permission()//56
            {
                Title = "نمایش لاگ ها"
            };
            Db.Permissions.Add(per);
        }
        
        public override void Down()
        {
        }
    }
}
