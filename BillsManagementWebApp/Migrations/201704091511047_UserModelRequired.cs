namespace BillsManagementWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserModelRequired : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "FirstName", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.Users", "LastName", c => c.String(nullable: false, maxLength: 150));
            AlterColumn("dbo.Users", "Username", c => c.String(nullable: false, maxLength: 40));
            AlterColumn("dbo.Users", "Password", c => c.String(nullable: false, maxLength: 300));
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "Email", c => c.String(maxLength: 100));
            AlterColumn("dbo.Users", "Password", c => c.String(maxLength: 300));
            AlterColumn("dbo.Users", "Username", c => c.String(maxLength: 40));
            DropColumn("dbo.Users", "LastName");
            DropColumn("dbo.Users", "FirstName");
        }
    }
}
