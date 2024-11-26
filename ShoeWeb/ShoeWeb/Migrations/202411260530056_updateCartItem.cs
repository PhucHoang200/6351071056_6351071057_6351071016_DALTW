namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateCartItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShoppingCartItems", "status", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShoppingCartItems", "status");
        }
    }
}
