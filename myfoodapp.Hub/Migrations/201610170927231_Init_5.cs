namespace myfoodapp.Hub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init_5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductionUnits", "picturePath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductionUnits", "picturePath");
        }
    }
}
