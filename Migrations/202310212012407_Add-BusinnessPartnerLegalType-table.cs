namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBusinnessPartnerLegalTypetable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusinnessPartnerLegalTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: false),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            Sql("insert into BusinnessPartnerLegalTypes (Id,Title) values (1,N'حقیقی'),(2,N'حقوقی'),(3,N'دولتی')");
        }

        public override void Down()
        {
            DropTable("dbo.BusinnessPartnerLegalTypes");
        }
    }
}
