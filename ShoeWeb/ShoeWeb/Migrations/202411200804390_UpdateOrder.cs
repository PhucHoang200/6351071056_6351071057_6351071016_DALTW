namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateOrder : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ShoppingCarts", "Status");
            DropColumn("dbo.ShoppingCarts", "Code");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShoppingCarts", "Code", c => c.String(nullable: false));
            AddColumn("dbo.ShoppingCarts", "Status", c => c.Int(nullable: false));
        }
    }
}
