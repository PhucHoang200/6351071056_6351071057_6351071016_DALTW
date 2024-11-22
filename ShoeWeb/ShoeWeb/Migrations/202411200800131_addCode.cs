namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShoppingCarts", "Code", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShoppingCarts", "Code");
        }
    }
}
