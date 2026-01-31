namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class password_needed_charcters : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SecuritySettings", "UseLowerCaseInPassword", c => c.Boolean(nullable: false,defaultValue:true));
            AddColumn("dbo.SecuritySettings", "UseUpperCaseInPassword", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.SecuritySettings", "UseNumbersInPassword", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.SecuritySettings", "UseSpecialCharactersInPassword", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SecuritySettings", "UseSpecialCharactersInPassword");
            DropColumn("dbo.SecuritySettings", "UseNumbersInPassword");
            DropColumn("dbo.SecuritySettings", "UseUpperCaseInPassword");
            DropColumn("dbo.SecuritySettings", "UseLowerCaseInPassword");
        }
    }
}
