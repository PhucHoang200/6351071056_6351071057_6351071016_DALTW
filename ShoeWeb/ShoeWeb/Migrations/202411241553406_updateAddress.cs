namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "TinhThanh", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "QuanHuyen", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "PhuongXa", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "PhuongXa");
            DropColumn("dbo.Orders", "QuanHuyen");
            DropColumn("dbo.Orders", "TinhThanh");
        }
    }
}
