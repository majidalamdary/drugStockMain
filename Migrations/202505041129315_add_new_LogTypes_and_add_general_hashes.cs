namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models.Constancts;
    using DrugStockWeb.Models.LogSystem;
    using DrugStockWeb.Models;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_new_LogTypes_and_add_general_hashes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GeneralHashes", "BlackListIp", c => c.Binary());




        }
        
        public override void Down()
        {
            DropColumn("dbo.GeneralHashes", "BlackListIp");
        }
    }
}
