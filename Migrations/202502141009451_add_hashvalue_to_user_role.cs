namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_hashvalue_to_user_role : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUserRoles", "HashValue", c => c.Binary());
            AddColumn("dbo.AspNetUserRoles", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUserRoles", "Discriminator");
            DropColumn("dbo.AspNetUserRoles", "HashValue");
        }
    }
}
