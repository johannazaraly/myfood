namespace myfoodapp.Hub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial_2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProductionUnits", "locationLatitude", c => c.Double(nullable: false));
            AlterColumn("dbo.ProductionUnits", "locationLongitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductionUnits", "locationLongitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.ProductionUnits", "locationLatitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
