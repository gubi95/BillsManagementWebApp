namespace BillsManagementWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductCategory_NewProp_UserOwnerId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductCategories", "UserOwnerId", c => c.Int(nullable: false));
            CreateIndex("dbo.ProductCategories", "UserOwnerId");
            AddForeignKey("dbo.ProductCategories", "UserOwnerId", "dbo.Users", "UserID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductCategories", "UserOwnerId", "dbo.Users");
            DropIndex("dbo.ProductCategories", new[] { "UserOwnerId" });
            DropColumn("dbo.ProductCategories", "UserOwnerId");
        }
    }
}
