namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUserId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "userId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Orders", "userId");
            AddForeignKey("dbo.Orders", "userId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "userId", "dbo.AspNetUsers");
            DropIndex("dbo.Orders", new[] { "userId" });
            DropColumn("dbo.Orders", "userId");
        }
    }
}
