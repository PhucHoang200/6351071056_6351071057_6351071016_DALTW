namespace ShoeWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabase : DbMigration
    {
        public override void Up()
        {
          
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.wards",
                c => new
                    {
                        code = c.String(nullable: false, maxLength: 128),
                        name = c.String(),
                        name_en = c.String(),
                        full_name = c.String(),
                        full_name_en = c.String(),
                        code_name = c.String(),
                        district_code = c.String(maxLength: 128),
                        administrative_unit_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.code);
            
            CreateTable(
                "dbo.provinces",
                c => new
                    {
                        code = c.String(nullable: false, maxLength: 128),
                        name = c.String(),
                        name_en = c.String(),
                        full_name = c.String(),
                        full_name_en = c.String(),
                        code_name = c.String(),
                        administrative_unit_id = c.Int(nullable: false),
                        administrative_region_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.code);
            
            CreateTable(
                "dbo.districts",
                c => new
                    {
                        code = c.String(nullable: false, maxLength: 128),
                        name = c.String(),
                        name_en = c.String(),
                        full_name = c.String(),
                        full_name_en = c.String(),
                        code_name = c.String(),
                        province_code = c.String(maxLength: 128),
                        administrative_unit_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.code);
            
            CreateTable(
                "dbo.administrative_units",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        full_name = c.String(),
                        full_name_en = c.String(),
                        short_name = c.String(),
                        short_name_en = c.String(),
                        code_name = c.String(),
                        code_name_en = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.administrative_regions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        name_en = c.String(),
                        code_en = c.String(),
                        code_name_en = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateIndex("dbo.wards", "administrative_unit_id");
            CreateIndex("dbo.wards", "district_code");
            CreateIndex("dbo.provinces", "administrative_region_id");
            CreateIndex("dbo.provinces", "administrative_unit_id");
            CreateIndex("dbo.districts", "administrative_unit_id");
            CreateIndex("dbo.districts", "province_code");
            AddForeignKey("dbo.wards", "district_code", "dbo.districts", "code");
            AddForeignKey("dbo.wards", "administrative_unit_id", "dbo.administrative_units", "id", cascadeDelete: true);
            AddForeignKey("dbo.districts", "province_code", "dbo.provinces", "code");
            AddForeignKey("dbo.provinces", "administrative_unit_id", "dbo.administrative_units", "id", cascadeDelete: true);
            AddForeignKey("dbo.provinces", "administrative_region_id", "dbo.administrative_regions", "id", cascadeDelete: true);
            AddForeignKey("dbo.districts", "administrative_unit_id", "dbo.administrative_units", "id", cascadeDelete: true);
        }
    }
}
