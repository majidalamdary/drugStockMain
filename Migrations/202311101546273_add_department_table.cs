using System.Linq;
using DrugStockWeb.Models.Account;
using Microsoft.AspNet.Identity;

namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Helper;
    using DrugStockWeb.Models;
    using System;
    using System.Data.Entity.Migrations;

    public partial class add_department_table : DbMigration
    {

        public override void Up()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Title = c.String(),
                })
                .PrimaryKey(t => t.Id);

            AddColumn("dbo.AspNetUsers", "Mobile", c => c.String());
            AddColumn("dbo.AspNetUsers", "DepartmentId", c => c.Guid(nullable: false));
            CreateIndex("dbo.AspNetUsers", "DepartmentId");
            AddForeignKey("dbo.AspNetUsers", "DepartmentId", "dbo.Departments", "Id", cascadeDelete: false);

            Sql("insert into Departments (Id,Title) values ('" + SequentialGuidGenerator.NewSequentialGuid() + "',N'مدیریت'),('" + SequentialGuidGenerator.NewSequentialGuid() + "', N'بخش مخدر')");

        }

        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.AspNetUsers", new[] { "DepartmentId" });
            DropColumn("dbo.AspNetUsers", "DepartmentId");
            DropColumn("dbo.AspNetUsers", "Mobile");
            DropTable("dbo.Departments");
        }
    }
}
