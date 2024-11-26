namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAllTable : DbMigration
    {
        public override void Up()
        {
          
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
            
        
        }
        
        public override void Down()
        {
         
            DropForeignKey("dbo.OTPs", "UserId", "dbo.AspNetUsers");
           
        }
    }
}
