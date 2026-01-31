using System.Linq;
using DevExpress.Data.WcfLinq.Helpers;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.BusinessPartner;
using DrugStockWeb.Models.Constanct;

namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addInvoiceReturnBusinnessPArtnerId : DbMigration
    {
        public MainDbContext Db = new MainDbContext();

        public override void Up()
        {
            var businnessPartnerGroup = new BusinnessPartnerGroup()
            {
                Id = Define.BussinesPartnerGroupForReturnInvoiceId,
                BusinnessPartnerLegalTypeId = BusinnessPartnerLegalTypeValues.Haghighi,
                BusinnessPartnerStatusId = BusinnessPartnerStatus.خریدار,
                SellWithBuyPrice = false,
                Title = "سایر",
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            };

            Db.BusinnessPartnerGroups.Add(businnessPartnerGroup);
            Db.SaveChanges();

        }
        
        public override void Down()
        {
        }
    }
}
