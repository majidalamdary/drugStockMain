namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_hashvalue_to_role : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetRoles", "HashValue", c => c.Binary());
            AddColumn("dbo.AspNetRoles", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetRoles", "Discriminator");
            DropColumn("dbo.AspNetRoles", "HashValue");
        }
    }
}
