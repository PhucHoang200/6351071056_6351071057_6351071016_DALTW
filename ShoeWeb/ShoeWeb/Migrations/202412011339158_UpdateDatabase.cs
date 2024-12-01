namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabase : DbMigration
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
                "dbo.OrderDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Int(nullable: false),
                        numberSize = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false),
                        CustomerName = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        Address = c.String(nullable: false),
                        TinhThanh = c.String(nullable: false),
                        QuanHuyen = c.String(nullable: false),
                        PhuongXa = c.String(nullable: false),
                        StatusShipping = c.Int(nullable: false),
                        isPayment = c.Boolean(nullable: false),
                        isAccept = c.Boolean(nullable: false),
                        Email = c.String(),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Int(nullable: false),
                        TypePayment = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        userId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.userId)
                .Index(t => t.userId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
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
                "dbo.Origins",
                c => new
                    {
                        idOrigin = c.Int(nullable: false, identity: true),
                        nameCountry = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.idOrigin);
            
            CreateTable(
                "dbo.OTPs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        OTPCode = c.String(nullable: false, maxLength: 6),
                        ExpiryDate = c.DateTime(nullable: false),
                        IsUsed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.ShoppingCartItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Int(nullable: false),
                        numberSize = c.Single(nullable: false),
                        status = c.Boolean(nullable: false),
                        UnitPriceDb = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ShoppingCartId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.ShoppingCarts", t => t.ShoppingCartId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.ShoppingCartId);
            
            CreateTable(
                "dbo.ShoppingCarts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SizeOfProducts", "sizeId", "dbo.Sizes");
            DropForeignKey("dbo.SizeOfProducts", "productId", "dbo.Products");
            DropForeignKey("dbo.ShoppingCarts", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ShoppingCartItems", "ShoppingCartId", "dbo.ShoppingCarts");
            DropForeignKey("dbo.ShoppingCartItems", "ProductId", "dbo.Products");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.OTPs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrderDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "idOrigin", "dbo.Origins");
            DropForeignKey("dbo.Products", "cateId", "dbo.Categories");
            DropForeignKey("dbo.Products", "brandId", "dbo.Brands");
            DropForeignKey("dbo.OrderDetails", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "userId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.SizeOfProducts", new[] { "sizeId" });
            DropIndex("dbo.SizeOfProducts", new[] { "productId" });
            DropIndex("dbo.ShoppingCarts", new[] { "UserId" });
            DropIndex("dbo.ShoppingCartItems", new[] { "ShoppingCartId" });
            DropIndex("dbo.ShoppingCartItems", new[] { "ProductId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.OTPs", new[] { "UserId" });
            DropIndex("dbo.Products", new[] { "idOrigin" });
            DropIndex("dbo.Products", new[] { "brandId" });
            DropIndex("dbo.Products", new[] { "cateId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Orders", new[] { "userId" });
            DropIndex("dbo.OrderDetails", new[] { "ProductId" });
            DropIndex("dbo.OrderDetails", new[] { "OrderId" });
            DropTable("dbo.Sizes");
            DropTable("dbo.SizeOfProducts");
            DropTable("dbo.ShoppingCarts");
            DropTable("dbo.ShoppingCartItems");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.OTPs");
            DropTable("dbo.Origins");
            DropTable("dbo.Products");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.Categories");
            DropTable("dbo.Brands");
        }
    }
}
