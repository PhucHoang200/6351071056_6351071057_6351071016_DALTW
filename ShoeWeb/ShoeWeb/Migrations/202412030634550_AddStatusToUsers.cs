namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatusToUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Status", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Status");
        }
    }
}
