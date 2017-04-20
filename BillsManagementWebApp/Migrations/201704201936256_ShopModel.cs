namespace BillsManagementWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShopModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Shops",
                c => new
                    {
                        ShopID = c.Int(nullable: false, identity: true),
                        ShopName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.ShopID);
            
            AddColumn("dbo.Bills", "Shop_ShopID", c => c.Int(nullable: false));
            CreateIndex("dbo.Bills", "Shop_ShopID");
            AddForeignKey("dbo.Bills", "Shop_ShopID", "dbo.Shops", "ShopID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bills", "Shop_ShopID", "dbo.Shops");
            DropIndex("dbo.Bills", new[] { "Shop_ShopID" });
            DropColumn("dbo.Bills", "Shop_ShopID");
            DropTable("dbo.Shops");
        }
    }
}
