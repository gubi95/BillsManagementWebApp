namespace BillsManagementWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Restore : DbMigration
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
            
            AddColumn("dbo.Bills", "User_UserID", c => c.Int());
            AddColumn("dbo.BillEntries", "Category_ProductCategoryID", c => c.Int(nullable: false));
            CreateIndex("dbo.BillEntries", "Category_ProductCategoryID");
            CreateIndex("dbo.Bills", "User_UserID");
            AddForeignKey("dbo.BillEntries", "Category_ProductCategoryID", "dbo.ProductCategories", "ProductCategoryID", cascadeDelete: true);
            AddForeignKey("dbo.Bills", "User_UserID", "dbo.Users", "UserID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bills", "User_UserID", "dbo.Users");
            DropForeignKey("dbo.BillEntries", "Category_ProductCategoryID", "dbo.ProductCategories");
            DropIndex("dbo.Bills", new[] { "User_UserID" });
            DropIndex("dbo.BillEntries", new[] { "Category_ProductCategoryID" });
            DropColumn("dbo.BillEntries", "Category_ProductCategoryID");
            DropColumn("dbo.Bills", "User_UserID");
            DropTable("dbo.ProductCategories");
        }
    }
}
