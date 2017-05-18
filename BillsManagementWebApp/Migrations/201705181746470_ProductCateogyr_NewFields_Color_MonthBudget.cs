namespace BillsManagementWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductCateogyr_NewFields_Color_MonthBudget : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductCategories", "Color", c => c.String());
            AddColumn("dbo.ProductCategories", "MonthBudget", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductCategories", "MonthBudget");
            DropColumn("dbo.ProductCategories", "Color");
        }
    }
}
