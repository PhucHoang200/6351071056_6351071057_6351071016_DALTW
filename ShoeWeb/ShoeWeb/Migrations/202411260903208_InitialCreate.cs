namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "isPayment", c => c.Boolean(nullable: false));
            AddColumn("dbo.Orders", "isAccept", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "isAccept");
            DropColumn("dbo.Orders", "isPayment");
        }
    }
}
