namespace BillsManagementWebApp.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Collections.Generic;

    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext() : base("name=defConnString")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDBContext, BillsManagementWebApp.Migrations.Configuration>("defConnString"));
        }

        public DbSet<User> Users { get; set; }
    }
}