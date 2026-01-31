namespace DrugStockWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addhashfields : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GenralHashes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserModel = c.Binary(),
                        PermissionModel = c.Binary(),
                        UserRoleModel = c.Binary(),
                        RoleModel = c.Binary(),
                        OnlineUserModel = c.Binary(),
                        PermisionInRoleModel = c.Binary(),
                        StoreInUserModel = c.Binary(),
                        HashValue = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUsers", "HashValue", c => c.Binary());
            AddColumn("dbo.StoreInUsers", "HashValue", c => c.Binary());
            AddColumn("dbo.OnlineUsers", "HashValue", c => c.Binary());
            AddColumn("dbo.PermissionInRoles", "HashValue", c => c.Binary());
            AddColumn("dbo.Permissions", "HashValue", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Permissions", "HashValue");
            DropColumn("dbo.PermissionInRoles", "HashValue");
            DropColumn("dbo.OnlineUsers", "HashValue");
            DropColumn("dbo.StoreInUsers", "HashValue");
            DropColumn("dbo.AspNetUsers", "HashValue");
            DropTable("dbo.GenralHashes");
        }
    }
}
