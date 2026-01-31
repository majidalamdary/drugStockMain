namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class addBusinnessPartnerTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusinnessPartners",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    CompanyName = c.String(),
                    FirstName = c.String(),
                    LastName = c.String(),
                    FatherName = c.String(),
                    Telphone = c.String(),
                    Mobile = c.String(),
                    MelliCode = c.String(),
                    Birthdate = c.DateTime(nullable: false),
                    Address = c.String(),
                    PostalCode = c.String(),
                    HamrahFirstName = c.String(),
                    HamrahLastName = c.String(),
                    HamrahFatherName = c.String(),
                    HamrahMelliCode = c.String(),
                    HamrahMobile = c.String(),
                    EconomicalCode  = c.String(),
                    HamrahTel = c.String(),
                    HamrahAddress = c.String(),
                    HamrahBirthDate = c.DateTime(nullable: false),
                    CreatorUserId = c.String(maxLength: 128),
                    CreateDate = c.DateTime(nullable: false),
                    UpdateDate = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorUserId)
                .Index(t => t.CreatorUserId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.BusinnessPartners", "CreatorUserId", "dbo.AspNetUsers");
            DropIndex("dbo.BusinnessPartners", new[] { "CreatorUserId" });
            DropTable("dbo.BusinnessPartners");
        }
    }
}
