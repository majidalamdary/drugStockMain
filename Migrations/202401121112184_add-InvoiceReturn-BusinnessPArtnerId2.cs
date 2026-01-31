namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Helper;
    using DrugStockWeb.Models;
    using DrugStockWeb.Models.BusinessPartner;
    using DrugStockWeb.Models.Constanct;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addInvoiceReturnBusinnessPArtnerId2 : DbMigration
    {
        public MainDbContext Db = new MainDbContext();
        public override void Up()
        {
            var businnessPartner = new BusinnessPartner()
            {
                Id = Define.BussinesPartnerForReturnInvoiceId,
                FirstName = "حواله",
                LastName = "مرجوعی",
                BusinnessPartnerGroupId = Define.BussinesPartnerGroupForReturnInvoiceId,
                Birthdate = DateTime.Now,
                HamrahBirthDate = DateTime.Now,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            };

            Db.BusinnessPartners.Add(businnessPartner);
            Db.SaveChanges();

        }

        public override void Down()
        {
        }
    }
}
