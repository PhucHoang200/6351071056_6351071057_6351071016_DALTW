namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModels : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderDetails", "numberSize", c => c.Single(nullable: false));
            AddColumn("dbo.Orders", "TinhThanh", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "QuanHuyen", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "PhuongXa", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "isPayment", c => c.Boolean(nullable: false));
            AddColumn("dbo.Orders", "isAccept", c => c.Boolean(nullable: false));
            AddColumn("dbo.ShoppingCartItems", "status", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShoppingCartItems", "status");
            DropColumn("dbo.Orders", "isAccept");
            DropColumn("dbo.Orders", "isPayment");
            DropColumn("dbo.Orders", "PhuongXa");
            DropColumn("dbo.Orders", "QuanHuyen");
            DropColumn("dbo.Orders", "TinhThanh");
            DropColumn("dbo.OrderDetails", "numberSize");
        }
    }
}
