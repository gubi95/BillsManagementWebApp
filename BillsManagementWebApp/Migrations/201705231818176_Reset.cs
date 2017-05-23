namespace BillsManagementWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Reset : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BillEntries",
                c => new
                    {
                        BillEntryID = c.Int(nullable: false, identity: true),
                        ProductName = c.String(nullable: false, maxLength: 100),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Double(nullable: false),
                        Bill_BillID = c.Int(),
                        Category_ProductCategoryID = c.Int(),
                    })
                .PrimaryKey(t => t.BillEntryID)
                .ForeignKey("dbo.Bills", t => t.Bill_BillID)
                .ForeignKey("dbo.ProductCategories", t => t.Category_ProductCategoryID)
                .Index(t => t.Bill_BillID)
                .Index(t => t.Category_ProductCategoryID);
            
            CreateTable(
                "dbo.ProductCategories",
                c => new
                    {
                        ProductCategoryID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Color = c.String(),
                        MonthBudget = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UserOwnerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductCategoryID)
                .ForeignKey("dbo.Users", t => t.UserOwnerId, cascadeDelete: true)
                .Index(t => t.UserOwnerId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 40),
                        Password = c.String(nullable: false, maxLength: 300),
                        Email = c.String(nullable: false, maxLength: 100),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        LastName = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateTable(
                "dbo.Bills",
                c => new
                    {
                        BillID = c.Int(nullable: false, identity: true),
                        PurchaseDate = c.DateTime(nullable: false, storeType: "date"),
                        Shop_ShopID = c.Int(nullable: false),
                        User_UserID = c.Int(),
                    })
                .PrimaryKey(t => t.BillID)
                .ForeignKey("dbo.Shops", t => t.Shop_ShopID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_UserID)
                .Index(t => t.Shop_ShopID)
                .Index(t => t.User_UserID);
            
            CreateTable(
                "dbo.Shops",
                c => new
                    {
                        ShopID = c.Int(nullable: false, identity: true),
                        ShopName = c.String(nullable: false, maxLength: 100),
                        UserOwner_UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ShopID)
                .ForeignKey("dbo.Users", t => t.UserOwner_UserID, cascadeDelete: true)
                .Index(t => t.UserOwner_UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BillEntries", "Category_ProductCategoryID", "dbo.ProductCategories");
            DropForeignKey("dbo.ProductCategories", "UserOwnerId", "dbo.Users");
            DropForeignKey("dbo.Bills", "User_UserID", "dbo.Users");
            DropForeignKey("dbo.Bills", "Shop_ShopID", "dbo.Shops");
            DropForeignKey("dbo.Shops", "UserOwner_UserID", "dbo.Users");
            DropForeignKey("dbo.BillEntries", "Bill_BillID", "dbo.Bills");
            DropIndex("dbo.Shops", new[] { "UserOwner_UserID" });
            DropIndex("dbo.Bills", new[] { "User_UserID" });
            DropIndex("dbo.Bills", new[] { "Shop_ShopID" });
            DropIndex("dbo.ProductCategories", new[] { "UserOwnerId" });
            DropIndex("dbo.BillEntries", new[] { "Category_ProductCategoryID" });
            DropIndex("dbo.BillEntries", new[] { "Bill_BillID" });
            DropTable("dbo.Shops");
            DropTable("dbo.Bills");
            DropTable("dbo.Users");
            DropTable("dbo.ProductCategories");
            DropTable("dbo.BillEntries");
        }
    }
}
