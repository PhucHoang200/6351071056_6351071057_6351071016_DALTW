namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addRoleToDb : DbMigration
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
                "dbo.Origins",
                c => new
                    {
                        idOrigin = c.Int(nullable: false, identity: true),
                        nameCountry = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.idOrigin);
            
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
                        createdDate = c.DateTime(nullable: false),
                        updatedDate = c.DateTime(nullable: false),
                        cateId = c.Int(nullable: false),
                        brandId = c.Int(nullable: false),
                        idOrigin = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.productId)
                .ForeignKey("dbo.Brands", t => t.brandId, cascadeDelete: true)
                .ForeignKey("dbo.Categories", t => t.cateId, cascadeDelete: true)
                .ForeignKey("dbo.Origins", t => t.idOrigin, cascadeDelete: true)
                .Index(t => t.cateId)
                .Index(t => t.brandId)
                .Index(t => t.idOrigin);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        NameRole = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.SizeOfProducts",
                c => new
                    {
                        productId = c.Int(nullable: false),
                        sizeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.productId, t.sizeId })
                .ForeignKey("dbo.Products", t => t.productId, cascadeDelete: true)
                .ForeignKey("dbo.Sizes", t => t.sizeId, cascadeDelete: true)
                .Index(t => t.productId)
                .Index(t => t.sizeId);
            
            CreateTable(
                "dbo.Sizes",
                c => new
                    {
                        sizeId = c.Int(nullable: false, identity: true),
                        numberSize = c.Single(nullable: false),
                        gender = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.sizeId);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        IdRole = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.IdRole, t.UserId })
                .ForeignKey("dbo.Roles", t => t.IdRole, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.IdRole)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        userId = c.Int(nullable: false, identity: true),
                        userName = c.String(nullable: false, maxLength: 100),
                        name = c.String(nullable: false, maxLength: 100),
                        email = c.String(nullable: false, maxLength: 100),
                        password = c.String(nullable: false, maxLength: 100),
                        phoneNumber = c.String(nullable: false, maxLength: 30),
                        randomKey = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.userId)
                .Index(t => t.userName, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "IdRole", "dbo.Roles");
            DropForeignKey("dbo.SizeOfProducts", "sizeId", "dbo.Sizes");
            DropForeignKey("dbo.SizeOfProducts", "productId", "dbo.Products");
            DropForeignKey("dbo.Products", "idOrigin", "dbo.Origins");
            DropForeignKey("dbo.Products", "cateId", "dbo.Categories");
            DropForeignKey("dbo.Products", "brandId", "dbo.Brands");
            DropIndex("dbo.Users", new[] { "userName" });
            DropIndex("dbo.UserRoles", new[] { "UserId" });
            DropIndex("dbo.UserRoles", new[] { "IdRole" });
            DropIndex("dbo.SizeOfProducts", new[] { "sizeId" });
            DropIndex("dbo.SizeOfProducts", new[] { "productId" });
            DropIndex("dbo.Products", new[] { "idOrigin" });
            DropIndex("dbo.Products", new[] { "brandId" });
            DropIndex("dbo.Products", new[] { "cateId" });
            DropTable("dbo.Users");
            DropTable("dbo.UserRoles");
            DropTable("dbo.Sizes");
            DropTable("dbo.SizeOfProducts");
            DropTable("dbo.Roles");
            DropTable("dbo.Products");
            DropTable("dbo.Origins");
            DropTable("dbo.Categories");
            DropTable("dbo.Brands");
        }
    }
}
