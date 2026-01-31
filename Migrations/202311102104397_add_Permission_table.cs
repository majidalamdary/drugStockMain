namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_Permission_table : DbMigration
    {
        public override void Up()
        {
            // DropForeignKey("dbo.AspNetUsers", "Course_Id", "dbo.Courses");
            // DropIndex("dbo.AspNetUsers", new[] { "Course_Id" });
            CreateTable(
                "dbo.PermissionInRoles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PermissionId = c.Int(nullable: false),
                        RoleId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Permissions", t => t.PermissionId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .Index(t => t.PermissionId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Permissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            // DropColumn("dbo.AspNetUsers", "Course_Id");
            // DropTable("dbo.Courses");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(),
                        Credit = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUsers", "Course_Id", c => c.Int());
            DropForeignKey("dbo.PermissionInRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.PermissionInRoles", "PermissionId", "dbo.Permissions");
            DropIndex("dbo.PermissionInRoles", new[] { "RoleId" });
            DropIndex("dbo.PermissionInRoles", new[] { "PermissionId" });
            DropTable("dbo.Permissions");
            DropTable("dbo.PermissionInRoles");
            CreateIndex("dbo.AspNetUsers", "Course_Id");
            AddForeignKey("dbo.AspNetUsers", "Course_Id", "dbo.Courses", "Id");
        }
    }
}
