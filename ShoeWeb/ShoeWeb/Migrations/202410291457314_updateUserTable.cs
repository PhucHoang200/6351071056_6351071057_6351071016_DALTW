namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateUserTable : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Users", "userName", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", new[] { "userName" });
        }
    }
}
