namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class address : DbMigration
    {
        public override void Up()
        {
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.wards", "district_code", "dbo.districts");
            DropForeignKey("dbo.wards", "administrative_unit_id", "dbo.administrative_units");
            DropForeignKey("dbo.districts", "province_code", "dbo.provinces");
            DropForeignKey("dbo.provinces", "administrative_unit_id", "dbo.administrative_units");
            DropForeignKey("dbo.provinces", "administrative_region_id", "dbo.administrative_regions");
            DropForeignKey("dbo.districts", "administrative_unit_id", "dbo.administrative_units");
            DropIndex("dbo.wards", new[] { "administrative_unit_id" });
            DropIndex("dbo.wards", new[] { "district_code" });
            DropIndex("dbo.provinces", new[] { "administrative_region_id" });
            DropIndex("dbo.provinces", new[] { "administrative_unit_id" });
            DropIndex("dbo.districts", new[] { "administrative_unit_id" });
            DropIndex("dbo.districts", new[] { "province_code" });
            DropTable("dbo.wards");
            DropTable("dbo.provinces");
            DropTable("dbo.districts");
            DropTable("dbo.administrative_units");
            DropTable("dbo.administrative_regions");
        }
    }
}
