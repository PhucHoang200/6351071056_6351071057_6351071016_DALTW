namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateOrder_02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "StatusShipping", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "StatusShipping");
        }
    }
}
