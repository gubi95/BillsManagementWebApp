namespace BillsManagementWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bills",
                c => new
                    {
                        BillID = c.Int(nullable: false, identity: true),
                        PurchaseDate = c.DateTime(nullable: false, storeType: "date"),
                        User_UserID = c.Int(),
                    })
                .PrimaryKey(t => t.BillID)
                .ForeignKey("dbo.Users", t => t.User_UserID)
                .Index(t => t.User_UserID);
            
            CreateTable(
                "dbo.BillEntries",
                c => new
                    {
                        BillEntryID = c.Int(nullable: false, identity: true),
                        ProductName = c.String(nullable: false, maxLength: 100),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Double(nullable: false),
                        Bill_BillID = c.Int(),
                    })
                .PrimaryKey(t => t.BillEntryID)
                .ForeignKey("dbo.Bills", t => t.Bill_BillID)
                .Index(t => t.Bill_BillID);
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bills", "User_UserID", "dbo.Users");
            DropForeignKey("dbo.BillEntries", "Bill_BillID", "dbo.Bills");
            DropIndex("dbo.BillEntries", new[] { "Bill_BillID" });
            DropIndex("dbo.Bills", new[] { "User_UserID" });
            DropTable("dbo.Users");
            DropTable("dbo.BillEntries");
            DropTable("dbo.Bills");
        }
    }
}
