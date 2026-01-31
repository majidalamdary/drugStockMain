using System.Linq;

namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSeedOstan : DbMigration
    {
        public MainDbContext Db = new MainDbContext();

        public override void Up()
        {
            var provinceRaw = System.IO.File.ReadAllText("ostan.json");
            var province = JsonConverter.ToOstanList(provinceRaw);

            foreach (var item in province.ToList())
            {
                 Db.Provinces.Add(item);
            }
            Db.SaveChanges();
            var cityRaw = System.IO.File.ReadAllText("shahrestan.json");
            var city= JsonConverter.ToCityList(cityRaw);

            foreach (var item in city.ToList())
            {
                 Db.Cities.Add(item);
            }

            Db.SaveChanges();
        }
        
        public override void Down()
        {
        }
    }
}
