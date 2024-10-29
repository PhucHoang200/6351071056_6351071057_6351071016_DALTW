namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addRoleToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        NameRole = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        IdRole = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.IdRole, t.UserId })
                .ForeignKey("dbo.Roles", t => t.IdRole, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.IdRole)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Users", "userName", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.Users", "role");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "role", c => c.String(nullable: false));
            DropForeignKey("dbo.UserRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "IdRole", "dbo.Roles");
            DropIndex("dbo.UserRoles", new[] { "UserId" });
            DropIndex("dbo.UserRoles", new[] { "IdRole" });
            DropColumn("dbo.Users", "userName");
            DropTable("dbo.UserRoles");
            DropTable("dbo.Roles");
        }
    }
}
