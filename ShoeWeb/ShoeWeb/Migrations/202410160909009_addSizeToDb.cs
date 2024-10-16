namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSizeToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Origins",
                c => new
                    {
                        idOrigin = c.Int(nullable: false, identity: true),
                        nameCountry = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.idOrigin);
            
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
                    })
                .PrimaryKey(t => t.sizeId);
            
            AddColumn("dbo.Products", "createdDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Products", "updatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Products", "idOrigin", c => c.Int(nullable: false));
            CreateIndex("dbo.Products", "idOrigin");
            AddForeignKey("dbo.Products", "idOrigin", "dbo.Origins", "idOrigin", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SizeOfProducts", "sizeId", "dbo.Sizes");
            DropForeignKey("dbo.SizeOfProducts", "productId", "dbo.Products");
            DropForeignKey("dbo.Products", "idOrigin", "dbo.Origins");
            DropIndex("dbo.SizeOfProducts", new[] { "sizeId" });
            DropIndex("dbo.SizeOfProducts", new[] { "productId" });
            DropIndex("dbo.Products", new[] { "idOrigin" });
            DropColumn("dbo.Products", "idOrigin");
            DropColumn("dbo.Products", "updatedDate");
            DropColumn("dbo.Products", "createdDate");
            DropTable("dbo.Sizes");
            DropTable("dbo.SizeOfProducts");
            DropTable("dbo.Origins");
        }
    }
}
