namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateOrderItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderDetails", "numberSize", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderDetails", "numberSize");
        }
    }
}
