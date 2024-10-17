namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addProductToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Brands",
                c => new
                    {
                        brandId = c.Int(nullable: false, identity: true),
                        brandName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.brandId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        cateId = c.Int(nullable: false, identity: true),
                        cateName = c.String(nullable: false, maxLength: 100),
                        cateDescription = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.cateId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        productId = c.Int(nullable: false, identity: true),
                        productName = c.String(nullable: false, maxLength: 100),
                        productDescription = c.String(nullable: false),
                        price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        quantity = c.Int(nullable: false),
                        image = c.String(nullable: false),
                        cateId = c.Int(nullable: false),
                        brandId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.productId)
                .ForeignKey("dbo.Brands", t => t.brandId, cascadeDelete: true)
                .ForeignKey("dbo.Categories", t => t.cateId, cascadeDelete: true)
                .Index(t => t.cateId)
                .Index(t => t.brandId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        userId = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 100),
                        email = c.String(nullable: false, maxLength: 100),
                        password = c.String(nullable: false, maxLength: 100),
                        phoneNumber = c.String(nullable: false, maxLength: 30),
                        randomKey = c.String(nullable: false),
                        role = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.userId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "cateId", "dbo.Categories");
            DropForeignKey("dbo.Products", "brandId", "dbo.Brands");
            DropIndex("dbo.Products", new[] { "brandId" });
            DropIndex("dbo.Products", new[] { "cateId" });
            DropTable("dbo.Users");
            DropTable("dbo.Products");
            DropTable("dbo.Categories");
            DropTable("dbo.Brands");
        }
    }
}
