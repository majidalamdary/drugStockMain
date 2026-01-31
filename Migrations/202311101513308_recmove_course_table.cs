namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recmove_course_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Invoices", "User_Id");
            AddForeignKey("dbo.Invoices", "User_Id", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.AspNetUsers", "Postition");
            DropColumn("dbo.AspNetUsers", "Department");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Department", c => c.String());
            AddColumn("dbo.AspNetUsers", "Postition", c => c.String());
            DropForeignKey("dbo.Invoices", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Invoices", new[] { "User_Id" });
            DropColumn("dbo.Invoices", "User_Id");
        }
    }
}
