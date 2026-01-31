namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatepermissiontables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PermissionInRoles", "PermissionId", "dbo.Permissions");
            DropIndex("dbo.PermissionInRoles", new[] { "PermissionId" });
            DropPrimaryKey("dbo.Permissions");
            AlterColumn("dbo.PermissionInRoles", "PermissionId", c => c.Int(nullable: false));
            AlterColumn("dbo.Permissions", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Permissions", "Id");
            CreateIndex("dbo.PermissionInRoles", "PermissionId");
            AddForeignKey("dbo.PermissionInRoles", "PermissionId", "dbo.Permissions", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PermissionInRoles", "PermissionId", "dbo.Permissions");
            DropIndex("dbo.PermissionInRoles", new[] { "PermissionId" });
            DropPrimaryKey("dbo.Permissions");
            AlterColumn("dbo.Permissions", "Id", c => c.Guid(nullable: false));
            AlterColumn("dbo.PermissionInRoles", "PermissionId", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.Permissions", "Id");
            CreateIndex("dbo.PermissionInRoles", "PermissionId");
            AddForeignKey("dbo.PermissionInRoles", "PermissionId", "dbo.Permissions", "Id", cascadeDelete: true);
        }
    }
}
