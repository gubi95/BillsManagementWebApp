namespace BillsManagementWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductCategory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductCategories",
                c => new
                    {
                        ProductCategoryID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.ProductCategoryID);
            
            AddColumn("dbo.BillEntries", "Category_ProductCategoryID", c => c.Int(nullable: false));
            CreateIndex("dbo.BillEntries", "Category_ProductCategoryID");
            AddForeignKey("dbo.BillEntries", "Category_ProductCategoryID", "dbo.ProductCategories", "ProductCategoryID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BillEntries", "Category_ProductCategoryID", "dbo.ProductCategories");
            DropIndex("dbo.BillEntries", new[] { "Category_ProductCategoryID" });
            DropColumn("dbo.BillEntries", "Category_ProductCategoryID");
            DropTable("dbo.ProductCategories");
        }
    }
}
