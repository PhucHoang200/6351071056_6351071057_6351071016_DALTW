namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSizeToDb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sizes", "gender", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sizes", "gender");
        }
    }
}
