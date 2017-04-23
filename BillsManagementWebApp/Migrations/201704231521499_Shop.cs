namespace BillsManagementWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Shop : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shops", "UserOwner_UserID", c => c.Int(nullable: false));
            CreateIndex("dbo.Shops", "UserOwner_UserID");
            AddForeignKey("dbo.Shops", "UserOwner_UserID", "dbo.Users", "UserID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Shops", "UserOwner_UserID", "dbo.Users");
            DropIndex("dbo.Shops", new[] { "UserOwner_UserID" });
            DropColumn("dbo.Shops", "UserOwner_UserID");
        }
    }
}
