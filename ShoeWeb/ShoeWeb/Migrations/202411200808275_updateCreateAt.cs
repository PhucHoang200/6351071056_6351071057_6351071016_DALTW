namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateCreateAt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "CreatedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "CreatedDate");
        }
    }
}
