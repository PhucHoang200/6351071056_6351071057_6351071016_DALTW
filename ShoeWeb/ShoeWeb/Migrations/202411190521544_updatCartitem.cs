namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatCartitem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShoppingCartItems", "UnitPriceDb", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShoppingCartItems", "UnitPriceDb");
        }
    }
}
