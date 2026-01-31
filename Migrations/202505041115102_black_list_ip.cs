namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class black_list_ip : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BlackListIps",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Ip = c.String(nullable: false),
                        HashValue = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.GeneralHashes", "OnlineUser", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GeneralHashes", "OnlineUser");
            DropTable("dbo.BlackListIps");
        }
    }
}
