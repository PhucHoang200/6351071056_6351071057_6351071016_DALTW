namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateOrderItem_1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OrderDetails", "numberSize", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OrderDetails", "numberSize", c => c.Int(nullable: false));
        }
    }
}
