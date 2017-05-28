namespace BillsManagementWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastModDate_ExtSysID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BillEntries", "LastModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.BillEntries", "ExternalSystemID", c => c.Int(nullable: false));
            AddColumn("dbo.ProductCategories", "LastModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.ProductCategories", "ExternalSystemID", c => c.Int(nullable: false));
            AddColumn("dbo.Bills", "LastModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Bills", "ExternalSystemID", c => c.Int(nullable: false));
            AddColumn("dbo.Shops", "LastModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Shops", "ExternalSystemID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shops", "ExternalSystemID");
            DropColumn("dbo.Shops", "LastModifiedDate");
            DropColumn("dbo.Bills", "ExternalSystemID");
            DropColumn("dbo.Bills", "LastModifiedDate");
            DropColumn("dbo.ProductCategories", "ExternalSystemID");
            DropColumn("dbo.ProductCategories", "LastModifiedDate");
            DropColumn("dbo.BillEntries", "ExternalSystemID");
            DropColumn("dbo.BillEntries", "LastModifiedDate");
        }
    }
}
